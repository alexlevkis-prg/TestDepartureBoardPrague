using Newtonsoft.Json;

namespace PIDTelegramBot.models;

public class InfoTextModel
{
    [JsonProperty(PropertyName = "display_type")]
    public string DisplayType {get; set;}

    [JsonProperty(PropertyName = "text")]
    public string Text {get; set;}

    [JsonProperty(PropertyName = "text_en")]
    public string EnglishText {get; set;}

    [JsonProperty(PropertyName = "related_stops")]
    public List<string> RelatedStops {get; set;}

    [JsonProperty(PropertyName = "valid_from")]
    public string ValidFrom {get; set;}

    [JsonProperty(PropertyName = "valid_to")]
    public string ValidTo {get; set;}
}
