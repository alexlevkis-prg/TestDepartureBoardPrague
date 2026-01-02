using Newtonsoft.Json;

public class DepartureVehicleModel
{
    [JsonProperty(PropertyName = "id")]
    public string Id {get; set;}

    [JsonProperty(PropertyName = "is_wheelchair_accessible")]
    public bool? IsWheelchairAccessible {get; set;}

    [JsonProperty(PropertyName = "is_air_conditioned")]
    public bool? IsAirConditioned {get; set;}

    [JsonProperty(PropertyName = "has_charger")]
    public bool? HasCharger {get; set;}
}