using Microsoft.Extensions.Configuration;

namespace PIDTelegramBot.helpers;

public sealed class ClientHelper
{
    private static IConfiguration configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
    
    private static readonly Lazy<HttpClient> instance = new Lazy<HttpClient>(() =>
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(configuration["PIDApiUrl"]);
            client.DefaultRequestHeaders.Add("X-Access-Token", configuration["apiToken"]);
            return client;
        });

    public static HttpClient Instance
    {
        get
        {
            return instance.Value;
        }
    }

    private ClientHelper()
    {
        
    }
}
