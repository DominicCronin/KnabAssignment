using System.Text.Json;

namespace ExchangeRates.Server.Models.CoinMarketCap
{
    public class CoinMarketCapQuote
    {
        public string? CurrencyId { get; set; }
        public string? TargetCurrencySymbol { get; set; }
        public CoinMarketCapStatus? Status { get; set; }
        public CoinMarketCapTargetCurrencyQuote? Quote { get; set; }
    }
}
