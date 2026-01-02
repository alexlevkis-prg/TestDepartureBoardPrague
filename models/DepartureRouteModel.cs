using Newtonsoft.Json;

public class DepartureRouteModel
{
    [JsonProperty(PropertyName = "type")]
    public string TransportType {get; set;}

    [JsonProperty(PropertyName = "short_name")]
    public string ShortName {get; set;}
}