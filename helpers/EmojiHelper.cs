using System.Text;

namespace PIDTelegramBot.helpers;

public static class EmojiHelper
{
    public static string GetTransportEmoji(string transportType)
    {
        return transportType switch
        {
            TransportType.Bus => "🚌",
            TransportType.Trolleybus => "🚎",
            TransportType.Tram => "🚊",
            TransportType.MetroA => "🚇",
            TransportType.MetroB => "🚇",
            TransportType.MetroC => "🚇",
            TransportType.Metro => "🚇",
            TransportType.Train => "🚋",
            TransportType.Ferry => "⛴",
            TransportType.Funicular => "🚠",
            TransportType.ExternalMiscellaneous => "🚐",
            _ => "🚐",
        };
    }

    public static string GetAdditionalVehicleInfoEmojis(DepartureVehicleModel vehicleModel)
    {
        var sb = new StringBuilder();
        if (vehicleModel.IsWheelchairAccessible.GetValueOrDefault(false))
        {
            sb.Append("♿️");
        }
        if (vehicleModel.IsAirConditioned.GetValueOrDefault(false))
        {
            sb.Append("❄️");
        }
        if (vehicleModel.HasCharger.GetValueOrDefault(false))
        {
            sb.Append("🔋");
        }
        return sb.Length > 0 ? sb.ToString() : string.Empty;
    }
}
