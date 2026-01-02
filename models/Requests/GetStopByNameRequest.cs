public class GetStopByNameRequest
{
    public string StopName {get; set;}
    public int Limit {get; set;}
    public int Offset {get; set;} = 0;
}