using Newtonsoft.Json;

public class FeaturePropertyModel
{
    [JsonProperty(PropertyName = "location_type")]
    public int LocationType {get; set;}

    [JsonProperty(PropertyName = "platform_code")]
    public string PlatformCode {get; set;}

    [JsonProperty(PropertyName = "stop_id")]
    public string StopId {get; set;}

    [JsonProperty(PropertyName = "stop_name")]
    public string StopName {get; set;}

    [JsonProperty(PropertyName = "wheelchair_boarding")]
    public bool WheelchairBoarding {get; set;}

    [JsonProperty(PropertyName = "zone_id")]
    public string ZoneId {get;set;}
}