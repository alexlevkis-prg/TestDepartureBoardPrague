public class DepartureBoardRequest
{
    public List<string> StopIds {get; set;} = new List<string>();
    public int Limit {get; set;} = 5;
    public string RouteShortNames {get; set;} = null;
    public int MinutesAfter {get; set;} = 60;
    public int MinutesBefore {get; set;} = 0;
}