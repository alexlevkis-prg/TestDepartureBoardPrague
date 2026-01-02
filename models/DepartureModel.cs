using Newtonsoft.Json;

public class DepartureModel
{
    [JsonProperty(PropertyName = "timestamp_scheduled")]
    public string ScheduledTime {get; set;}

    [JsonProperty(PropertyName = "timestamp_predicted")]
    public string PredictedTime {get; set;}

    [JsonProperty(PropertyName = "delay_seconds")]
    public int? Delay {get; set;}

    [JsonProperty(PropertyName = "minutes")]
    public int Minutes {get; set;}
}