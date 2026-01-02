using Microsoft.Extensions.Configuration;
using PIDTelegramBot.data;
using PIDTelegramBot.helpers;
using Telegram.Bot;
using Telegram.Bot.Types;

internal class Program
{
    private static int locationMessageId = 0;
    private static int textMessageId = 0;
    private static int suggestionStopMessageId = 0;
    private static long clientId = 0;

    private static IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    private static string rootPath = AppDomain.CurrentDomain.BaseDirectory + "\\data";
    private static void Main()
    {
        try
        {
            if (string.IsNullOrEmpty(configuration["clientId"]))
            {
                throw new ArgumentNullException("clientId", "ClientId parameter is not defined. Please set it in the settings.");
            }
            if (string.IsNullOrEmpty(configuration["PIDApiUrl"]))
            {
                throw new ArgumentNullException("PIDApiUrl", "PIDApiUrl parameter is not defined. Please set it in the settings.");
            }
            if (string.IsNullOrEmpty(configuration["apiToken"]))
            {
                throw new ArgumentNullException("apiToken", "apiToken parameter is not defined. Please set it in the settings.");
            }
            if (string.IsNullOrEmpty(configuration["dbFile"]))
            {
                throw new ArgumentNullException("dbFile", "dbFile parameter is not defined. Please set it in the settings.");
            }
            if (string.IsNullOrEmpty(configuration["clientToken"]))
            {
                throw new ArgumentNullException("clientToken", "clientToken parameter is not defined. Please set it in the settings.");
            }
            Host host = new Host(configuration["clientToken"]);
            clientId = long.Parse(configuration["clientId"]);
            host.Start();
            host.OnMessage += OnMessage;
            Console.ReadLine();

        }
        catch (Exception ex)
        {
            throw ex;
        }       
    }

    private static async void OnMessage(ITelegramBotClient client, Update update)
    {
        if (update.Message == null)
        {
            if (update.CallbackQuery != null)
            {
                var messageId = update.CallbackQuery.Message.Id;
                if (update.CallbackQuery.Data == "Platform:All")
                {
                    return;
                }
                else
                {
                    if (update.CallbackQuery.Data.StartsWith("Stop:"))
                    {
                        SendTelegramMessage(client, update, update.CallbackQuery.Data.Split(':').Last(), true);
                    }
                    else if(update.CallbackQuery.Data.StartsWith("Platform:"))
                    {
                        var parsed = update.CallbackQuery.Data.Split(';');
                        var stopsGtfs = parsed.FirstOrDefault(x => x.StartsWith("StopId:"))?.Split(':').Last().Split(',');
                        var httpClient = ClientHelper.Instance;
                        var pidService = PIDApiService.Instance;
                        var databaseService = DatabaseService.Instance;
                        var platformModels = databaseService.GetPlatformNameByCode(stopsGtfs, rootPath, configuration["dbFile"]);
                        var departureBoardRequest = RequestModelBuilder.CreateDepartureBoardRequest(stopsGtfs.ToList());
                        var departureBoard = await pidService.GetDepartureBoard(httpClient, departureBoardRequest);
                        var infoTexts = await pidService.GetStopInfoTexts(httpClient, stopsGtfs.ToList());
                        var departureBoardMessage = MessageBuilder.BuildStopDepartureBoardMessage(departureBoard, platformModels.First().Name, infoTexts);
                        await DeleteLocationMessages(client, update.CallbackQuery?.Message?.Chat.Id ?? clientId);
                        await client.SendLocation(update.CallbackQuery.Message.Chat.Id,
                            (double)platformModels.First().Latitude,
                            (double)platformModels.First().Longitude);
                        await client.SendMessage(update.CallbackQuery.Message.Chat.Id,
                            departureBoardMessage,
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                            replyMarkup: null
                        );
                    }
                }
                var data = update.CallbackQuery.Data;
            }
        }
        else if (update.Message?.Text == "/start")
        {
            await client.SendMessage(update.Message?.Chat.Id ?? clientId, MessageBuilder.BuildWelcomeMessage(), parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
        }
        else if (string.IsNullOrEmpty(update.Message?.Text))
        {
            return;
        }
        else
        {
            var departureBoardRequest = new GetStopByNameRequest()
            {
                StopName = update.Message?.Text ?? string.Empty,
            };
            var httpClient = ClientHelper.Instance;
            var pidService = PIDApiService.Instance;
            var databaseService = DatabaseService.Instance;
            var stops = databaseService.GetStopsByName(update.Message?.Text, rootPath, configuration["dbFile"]);
            if (stops.Count > 1)
            {
                var buttons = MessageBuilder.BuildStopsSuggestions(stops);
                var suggestionMessage = await client.SendMessage(update.Message?.Chat.Id ?? clientId,
                    $"Please select suggestions", 
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, 
                    replyMarkup: buttons);
                suggestionStopMessageId = suggestionMessage.Id;
            }
            else if (stops.Count == 1)
            {
                SendTelegramMessage(client, update, stops[0].Name, false);
            }
            else if (stops.Count == 0)
            {
                SendTelegramMessage(client, update, string.Empty, false);
            }
        }     
    }

    private static async void SendTelegramMessage(ITelegramBotClient client, Update update, string stopName, bool isUpdate = false)
    {
        if (string.IsNullOrEmpty(stopName))
        {
            await client.SendMessage(update.Message?.Chat.Id ?? clientId, 
                    $"Sorry but no any stops exist with typed text. Please try do another attempt with different text", 
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            return;
        }
        var databaseService = DatabaseService.Instance;
        var stopWithPlatforms = databaseService.GetStopWithPlatforms(stopName, rootPath, configuration["dbFile"]);
        stopWithPlatforms.Stops = stopWithPlatforms.Stops.ToList();
        if (stopWithPlatforms.Stops.Count > 2)
        {
            var buttons = MessageBuilder.BuildPlatformSuggestions(stopWithPlatforms.Stops);
            if (isUpdate)
            {
                var textMsg = await client.EditMessageText(update.CallbackQuery?.Message?.Chat.Id ?? clientId, 
                    update.CallbackQuery.Message.Id,
                    $"<b>{stopName}</b>: Please select stop",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                
                textMessageId = textMsg.MessageId;
                
                var locationMsg = await client.SendLocation(update.CallbackQuery?.Message?.Chat.Id ?? clientId, 
                    (double)stopWithPlatforms.Latitude,
                    (double)stopWithPlatforms.Longitude,
                    replyMarkup: buttons);
                locationMessageId = locationMsg.MessageId;
            }
            else
            {
                var textMsg = await client.SendMessage(update.Message?.Chat.Id ?? clientId, 
                    $"<b>{stopName}</b>: Please select stop", 
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                
                textMessageId = textMsg.MessageId;
                var locationMsg = await client.SendLocation(update.Message?.Chat.Id ?? clientId, 
                    (double)stopWithPlatforms.Latitude,
                    (double)stopWithPlatforms.Longitude,
                    replyMarkup: buttons);
                locationMessageId = locationMsg.MessageId;
            }
        }
        else
        {
            var stopsGtfs = new List<string>();
            foreach(var stop in stopWithPlatforms.Stops)
            {
                var split = stop.GtfsIdsString.Split(',');
                stopsGtfs.AddRange(split);
            }
            var httpClient = ClientHelper.Instance;
            var pidService = PIDApiService.Instance;
            var platformModels = databaseService.GetPlatformNameByCode(stopsGtfs.ToArray(), rootPath, configuration["dbFile"]);
            var departureBoardRequest = RequestModelBuilder.CreateDepartureBoardRequest(stopsGtfs);
            var departureBoard = await pidService.GetDepartureBoard(httpClient, departureBoardRequest);
            var infoTexts = await pidService.GetStopInfoTexts(httpClient, stopsGtfs.ToList());
            var departureBoardMessage = MessageBuilder.BuildStopDepartureBoardMessage(departureBoard, platformModels.First().Name, infoTexts);
            if (isUpdate)
            {
                await DeleteLocationMessages(client, update.CallbackQuery?.Message?.Chat.Id ?? clientId);
                await client.SendLocation(update.CallbackQuery?.Message?.Chat.Id ?? clientId,
                    (double)stopWithPlatforms.Latitude,
                    (double)stopWithPlatforms.Longitude);
                await client.SendMessage(update.CallbackQuery?.Message?.Chat.Id ?? clientId,
                    departureBoardMessage,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
            else
            {
                await client.SendLocation(update.CallbackQuery?.Message?.Chat.Id ?? clientId,
                    (double)stopWithPlatforms.Latitude,
                    (double)stopWithPlatforms.Longitude);
                await client.SendMessage(update.Message?.Chat.Id ?? clientId, 
                    departureBoardMessage, 
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
            }
        }
    }

    private static async Task DeleteLocationMessages(ITelegramBotClient client, long chatId)
    {
        if (locationMessageId > 0)
        {
            await client.DeleteMessage(chatId, locationMessageId);
            locationMessageId = 0;
        }
        if (textMessageId > 0)
        {
            await client.DeleteMessage(chatId, textMessageId);
            textMessageId = 0;
        }
        if (suggestionStopMessageId > 0)
        {
            await client.DeleteMessage(chatId, suggestionStopMessageId);
            suggestionStopMessageId = 0;
        }
    }
}