using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

public class Host
{
    private TelegramBotClient telegramBotClient;

    public Action<ITelegramBotClient, Update>? OnMessage;

    public Host(string token)
    {
        telegramBotClient = new TelegramBotClient(token);
    }

    public void Start()
    {
        telegramBotClient.StartReceiving(UpdateHandler, ErrorHandler);
    }

    private async Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        Console.WriteLine($"Error: {exception.Message}");
        await Task.CompletedTask;
    }

    private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        Console.WriteLine($"Message recieved: {update.Message?.Text ?? "[not a text]"}");
        OnMessage?.Invoke(client, update);
        await Task.CompletedTask;
    }
}