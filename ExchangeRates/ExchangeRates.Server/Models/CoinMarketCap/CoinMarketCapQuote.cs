using System.Text.Json;

namespace ExchangeRates.Server.Models.CoinMarketCap
{
    public record CoinMarketCapQuote
    {
        public required string CurrencyId { get; init; }
        public required string TargetCurrencySymbol { get; init; }
        public required CoinMarketCapStatus Status { get; init; }
        public required CoinMarketCapTargetCurrencyQuote Quote { get; init; }
    }
}
