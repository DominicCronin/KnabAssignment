using ExchangeRates.Server.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExchangeRates.Server.Services
{
    public class CoinMarketCapClientBase(IOptions<CoinMarketCapOptions> options)
    {

        protected static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        protected async Task<HttpResponseMessage> GetResponseFromAPI(string uri)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.Value.ApiKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return await client.GetAsync(uri);
        }
    }
}