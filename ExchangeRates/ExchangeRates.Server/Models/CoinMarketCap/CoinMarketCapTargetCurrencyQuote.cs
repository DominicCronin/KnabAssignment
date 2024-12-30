using System.Text.Json;

namespace ExchangeRates.Server.Models.CoinMarketCap
{
    public class CoinMarketCapTargetCurrencyQuote
    {
        public required double Price{ get; set; }
        public DateTimeOffset? LastUpdated { get; set; }

    }
}
