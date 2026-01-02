using Newtonsoft.Json;

public class FeatureModel
{
    [JsonProperty(PropertyName = "properties")]
    public FeaturePropertyModel Properties {get; set;}
}