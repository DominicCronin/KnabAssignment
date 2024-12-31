using System.Text.Json;

namespace ExchangeRates.Server.Models.CoinMarketCap
{
    public record CoinMarketCapTargetCurrencyQuote
    {
        public required double Price{ get; init; }
        public DateTimeOffset? LastUpdated { get; init; }

    }
}
