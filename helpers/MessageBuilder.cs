using System.Text;
using PIDTelegramBot.models;
using PIDTelegramBot.models.Responses;
using Telegram.Bot.Types.ReplyMarkups;

namespace PIDTelegramBot.helpers;

public static class MessageBuilder
{
    public static string BuildStopDepartureBoardMessage(DepartureBoardArrayModel model, string stopName, StopInfoTextResponse infoTextResponse)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<b>{stopName.ToUpper()}</b> 🚏");
        sb.AppendLine(string.Empty);
        if (infoTextResponse != null
            && infoTextResponse.InfoTexts != null 
            && infoTextResponse.InfoTexts.Count > 0
            && infoTextResponse.InfoTexts.Any(x => x.DisplayType == InfoTextDisplayType.General))
        {
            var generalInfos = infoTextResponse.InfoTexts?.Where(i => i.DisplayType == InfoTextDisplayType.General);
            PutInfoTexts(generalInfos, sb);           
            return sb.ToString();
        }
        foreach (var departurePlatform in model)
        {
            var platforms = departurePlatform.Select(x => new {x.Stop.Id, x.Stop.PlatformCode}).Distinct();
            foreach(var platform in platforms)
            {
                sb.AppendLine($"<b>Platform {platform.PlatformCode}</b>");
                var infoTexts = infoTextResponse?.InfoTexts?.Where(x => x.RelatedStops.Any(y => y == platform.Id));
                PutInfoTexts(infoTexts, sb);              
                var platformDepartures = departurePlatform.Where(x => x.Stop.PlatformCode == platform.PlatformCode);
                foreach (var platformDeparture in platformDepartures)
                {
                    sb.Append($"{EmojiHelper.GetTransportEmoji(platformDeparture.Route.TransportType)}");
                    sb.Append($"{EmojiHelper.GetAdditionalVehicleInfoEmojis(platformDeparture.Vehicle)}");
                    sb.Append("    ");
                    sb.Append($"<b>{platformDeparture.Route.ShortName}</b>  ->  ");
                    sb.Append($"<b>{platformDeparture.Trip.Headsign}</b>    ");
                    if (platformDeparture.Trip.IsCancelled.GetValueOrDefault(false))
                    {
                        sb.Append($"<b>cancelled</b>❌");
                    }
                    else
                    {
                        var delayInMinutes = Math.Round((decimal)platformDeparture.Departure.Delay.GetValueOrDefault(0) / 60, 0);
                        if (platformDeparture.Departure.Minutes == 0 && delayInMinutes == 0)
                        {
                            sb.Append($"<b>arrived</b>✅");
                        }
                        else if (platformDeparture.Departure.Minutes == 0 && delayInMinutes > 0)
                        {
                            sb.Append($"delaying for {delayInMinutes} {GetMinuteTitle(delayInMinutes)}");
                        }
                        else
                        {
                            sb.Append($"in {platformDeparture.Departure.Minutes} {GetMinuteTitle(platformDeparture.Departure.Minutes)} ");
                            if (delayInMinutes > 0)
                            {
                                sb.Append($"(delay: <i>{delayInMinutes} {GetMinuteTitle(delayInMinutes)}</i>)");
                            }                         
                        }
                    }               
                    sb.AppendLine();
                }
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static InlineKeyboardMarkup BuildStopsSuggestions(List<StopModel> stopModels)
    {
        var buttons = new List<List<InlineKeyboardButton>>();
        for (int i = 0; i < stopModels.Count; i++)
        {
            if (i % 2 == 0)
            {
                buttons.Add(new List<InlineKeyboardButton>());
                buttons.Last().Add(new InlineKeyboardButton()
                {
                    Text = stopModels[i].Name,
                    CallbackData = "Stop:" + stopModels[i].Name
                });
            }
            else
            {
                buttons.Last().Add(new InlineKeyboardButton()
                {
                    Text = stopModels[i].Name,
                    CallbackData = "Stop:" + stopModels[i].Name
                });
            }
        }
        var km = new InlineKeyboardMarkup(buttons.Select(x => x.ToArray()).ToArray());
        return km;
    }

    public static InlineKeyboardMarkup BuildPlatformSuggestions(List<PlatformModel> platforms)
    {
        var groups = platforms.GroupBy(g => g.TransportType);
        var buttons = new List<List<InlineKeyboardButton>>();
        foreach (var group in groups)
        {
            for(var i = 0; i < group.Count(); i++)
            {
                if (i % 2 == 0)
                {
                    buttons.Add(new List<InlineKeyboardButton>());
                    buttons.Last().Add(new InlineKeyboardButton()
                    {
                        Text = EmojiHelper.GetTransportEmoji(group.Key) + " Platform " + group.ElementAt(i).PlatformCode,
                        CallbackData = $"Platform:{group.ElementAt(i).PlatformCode};StopId:{group.ElementAt(i).GtfsIdsString}"
                    });
                }
                else
                {
                    buttons.Last().Add(new InlineKeyboardButton()
                    {
                        Text = EmojiHelper.GetTransportEmoji(group.Key) + " Platform " + group.ElementAt(i).PlatformCode,
                        CallbackData = $"Platform:{group.ElementAt(i).PlatformCode};StopId:{group.ElementAt(i).GtfsIdsString}"
                    });
                }
            }
        }
        var km = new InlineKeyboardMarkup(buttons.Select(x => x.ToArray()).ToArray());
        return km;
    }

    public static string BuildWelcomeMessage()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Welcome to the <b>Departure Board</b>. Prague bot.");
        sb.AppendLine("To start using it please type here the stop/station name in <b>Czech</b> language");
        sb.AppendLine("The bot will return you info about the nearest public transport for your stop/station");
        sb.AppendLine("If your stop/station has multiple transport types, the bot will suggest you to select particular one.");
        return sb.ToString();
    }

    private static void PutInfoTexts(IEnumerable<InfoTextModel> infoTextModels, StringBuilder stringBuilder)
    {
        if (infoTextModels != null && infoTextModels.Count() > 0)
        {
            var czechTexts = infoTextModels.Select(g => g.Text);
            var englishTexts = infoTextModels.Select(g => g.EnglishText);
            stringBuilder.Append("🇨🇿⚠️");
            foreach(var czechText in czechTexts)
            {
                stringBuilder.AppendLine($"{czechText}");
            }
            stringBuilder.AppendLine();
            stringBuilder.Append("🇬🇧⚠️");
            foreach(var englishText in englishTexts)
            {
                stringBuilder.AppendLine($"{englishText}");
            }
            stringBuilder.AppendLine();
        }
    }

    private static string GetMinuteTitle(decimal minutes)
    {
        return minutes > 1 ? "minutes" : "minute";
    }
}
