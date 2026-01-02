using Newtonsoft.Json;
using PIDTelegramBot.models.Responses;

public sealed class PIDApiService
{
    private static readonly Lazy<PIDApiService> apiService = new Lazy<PIDApiService>(() => new PIDApiService());

    public static PIDApiService Instance
    {
        get
        {
            return apiService.Value;
        }
    }

    private PIDApiService()
    {
        
    }

    public async Task<DepartureBoardArrayModel> GetDepartureBoard(HttpClient httpClient, DepartureBoardRequest departureBoardRequest)
    {
        try
        {
            var request = RequestBuilder.BuildDepartureBoardRequest(departureBoardRequest);
            using HttpResponseMessage responseMessage = await httpClient.GetAsync(request);
            responseMessage.EnsureSuccessStatusCode();
            var resultString = await responseMessage.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<DepartureBoardArrayModel>(resultString);
            return content;
        }
        catch (Exception ex)
        {
            throw ex;
        }       
    }

    public async Task<StopInfoTextResponse> GetStopInfoTexts(HttpClient httpClient, List<string> stopIds)
    {
        try
        {
            var request = RequestBuilder.BuildInfoTextRequest(stopIds);
            using HttpResponseMessage responseMessage = await httpClient.GetAsync(request);
            responseMessage.EnsureSuccessStatusCode();
            var resultString = await responseMessage.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<StopInfoTextResponse>(resultString);
            return content;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}