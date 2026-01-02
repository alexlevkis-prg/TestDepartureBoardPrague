using Newtonsoft.Json;

public class DepartureTripModel
{
    [JsonProperty(PropertyName = "id")]
    public string Id {get; set;}

    [JsonProperty(PropertyName = "headsign")]
    public string Headsign {get; set;}

    [JsonProperty(PropertyName = "is_canceled")]
    public bool? IsCancelled {get; set;}
}