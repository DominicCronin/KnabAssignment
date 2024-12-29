namespace ExchangeRates.Server.Options
{
    public class CoinMarketCapOptions
    {
        public const string Section = "CoinMarketCap";
        public string ApiKey { get; set; } = String.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string[] CurrencySymbols { get; set; } = [];
    }
}
