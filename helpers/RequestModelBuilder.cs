namespace PIDTelegramBot.helpers;

public static class RequestModelBuilder
{
    public static DepartureBoardRequest CreateDepartureBoardRequest(List<string> stops)
    {
        return new DepartureBoardRequest()
        {
            StopIds = stops
        };
    }
}
