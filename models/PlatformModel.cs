using Newtonsoft.Json;

namespace PIDTelegramBot.models;

public class PlatformModel
{
    public int Id { get; set; }

    [JsonProperty(PropertyName = "platform")]
    public string PlatformCode { get; set; }

    [JsonProperty(PropertyName = "altIdosName")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "zone")]
    public string Zone { get; set; }

    [JsonProperty(PropertyName = "mainTrafficType")]
    public string TransportType { get; set; }

    [JsonProperty(PropertyName = "lat")]
    public decimal Latitude { get; set; }

    [JsonProperty(PropertyName = "lon")]
    public decimal Longitude { get; set; }

    public string GtfsIdsString  { get; set; }

    public int StopId { get; set; }
}
