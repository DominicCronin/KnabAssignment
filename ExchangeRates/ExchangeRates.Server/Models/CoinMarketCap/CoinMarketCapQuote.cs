using System.Text.Json;

namespace ExchangeRates.Server.Models.CoinMarketCap
{
    public record CoinMarketCapQuote
    {
        public string? CurrencyId { get; init; }
        public string? TargetCurrencySymbol { get; init; }
        public CoinMarketCapStatus? Status { get; init; }
        public CoinMarketCapTargetCurrencyQuote? Quote { get; init; }
    }
}
