using Newtonsoft.Json;

public class GetStopByNameResponse
{
    [JsonProperty(PropertyName = "features")]
    public List<FeatureModel> FeatureModels {get; set;}
}