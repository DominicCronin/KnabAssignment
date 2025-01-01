using ExchangeRates.Server.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ExchangeRates.Server.Services
{
    public class CoinMarketCapClientBase()
    {

        protected static readonly JsonSerializerOptions _jsonSerializerOptions = new() { 
            PropertyNameCaseInsensitive = true, 
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            
        };
    }
}