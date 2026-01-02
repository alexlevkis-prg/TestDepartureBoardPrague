using System.Text;

public static class RequestBuilder
{
    public static string BuildDepartureBoardRequest(DepartureBoardRequest departureBoardRequest)
    {
        var sb = new StringBuilder();
        sb.Append("/v2/public/departureboards");
        if (departureBoardRequest.StopIds == null || departureBoardRequest.StopIds.Count == 0)
        {
            return string.Empty;
        }
        sb.Append("?");    
        for (int i = 0; i < departureBoardRequest.StopIds.Count; i++)
        {
            sb.Append("stopIds[]={\"");
            sb.Append($"{i}");
            sb.Append($"\": [");                     
            sb.Append($"\"{departureBoardRequest.StopIds[i]}\", ");
            sb.Remove(sb.Length - 2, 2);
            sb.Append("]}&");        
        }
        sb.Remove(sb.Length - 1, 1);
        if (departureBoardRequest.Limit > 0)
        {
            sb.Append($"&limit={departureBoardRequest.Limit}");
        }
        if (departureBoardRequest.RouteShortNames != null)
        {
            sb.Append($"&routeShortNames={departureBoardRequest.RouteShortNames[0]}");
        }
        if (departureBoardRequest.MinutesAfter > 0)
        {
            sb.Append($"&minutesAfter={departureBoardRequest.MinutesAfter}");
        }
        if (departureBoardRequest.MinutesBefore != 0)
        {
            sb.Append($"&minutesBefore={departureBoardRequest.MinutesBefore}");
        }
        return sb.ToString();
    }

    public static string BuildDepartureBoardRequest(List<string> stopIds)
    {
        var sb = new StringBuilder();
        sb.Append("/v2/public/departureboards");
        if (stopIds == null || stopIds.Count == 0)
        {
            return string.Empty;
        }
        sb.Append("?");
        for (int i = 0; i < stopIds.Count; i++)
        {
            sb.Append("stopIds[]={\"");
            sb.Append($"{i}");
            sb.Append($"\": [");
            foreach(var stopId in stopIds)
            {                        
                sb.Append($"\"{stopId}\", ");
            }          
            sb.Remove(sb.Length - 2, 2);
            sb.Append("]}&");
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append($"&limit={5}");
        sb.Append($"&minutesAfter={60}");
        return sb.ToString();
    }

    public static string BuildInfoTextRequest(List<string> stopIds)
    {
        var sb = new StringBuilder();
        sb.Append("/v2/pid/departureboards");
        sb.Append("?");
        foreach(var stopId in stopIds)
        {
            sb.Append($"ids[]={stopId}&");
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("&minutesBefore=10&minutesAfter=60&preferredTimezone=Europe_Prague&mode=departures&order=real&filter=routeOnce&limit=20&total=0&offset=0");
        return sb.ToString();
    }
    
    public static string BuildGetStopByNameRequest(GetStopByNameRequest getStopByNameRequest)
    {
        var sb = new StringBuilder();
        sb.Append("/v2/gtfs/stops");
        if (string.IsNullOrEmpty(getStopByNameRequest.StopName))
        {
            return string.Empty;
        }
        sb.Append($"?names[]={getStopByNameRequest.StopName}");
        if (getStopByNameRequest.Limit > 0)
        {
            sb.Append($"&limit={getStopByNameRequest.Limit}");
        }
        if (getStopByNameRequest.Offset > 0)
        {
            sb.Append($"&offset={getStopByNameRequest.Offset}");
        }
        return sb.ToString();
    }
}