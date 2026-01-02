using Newtonsoft.Json;

public class DepartureStopModel
{
    [JsonProperty(PropertyName = "id")]
    public string Id {get; set;}

    [JsonProperty(PropertyName = "sequence")]
    public int Sequence {get; set;}

    [JsonProperty(PropertyName = "platform_code")]
    public string PlatformCode {get; set;}
}