using System;
using Newtonsoft.Json;

namespace PIDTelegramBot.models;

public class StopModel
{
    public int Id { get; set;}

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set;}

    [JsonProperty(PropertyName = "municipality")]
    public string Municipality { get; set; }

    [JsonProperty(PropertyName = "avgLat")]
    public decimal Latitude { get; set; }

    [JsonProperty(PropertyName = "avgLon")]
    public decimal Longitude { get; set; }

    [JsonProperty(PropertyName = "stops")]
    public List<PlatformModel> Stops { get; set; }
}
