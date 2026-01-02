using Newtonsoft.Json;

public class DepartureBoardModel
{
    [JsonProperty(PropertyName = "departure")]
    public DepartureModel Departure {get; set;}

    [JsonProperty(PropertyName = "stop")]
    public DepartureStopModel Stop {get; set;}

    [JsonProperty(PropertyName = "route")]
    public DepartureRouteModel Route {get; set;}

    [JsonProperty(PropertyName = "trip")]
    public DepartureTripModel Trip {get; set;}

    [JsonProperty(PropertyName = "vehicle")]
    public DepartureVehicleModel Vehicle {get; set;}
}