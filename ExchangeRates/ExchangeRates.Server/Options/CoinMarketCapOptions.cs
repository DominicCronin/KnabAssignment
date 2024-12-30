namespace ExchangeRates.Server.Options
{
    public class CoinMarketCapOptions
    {
        public const string Section = "CoinMarketCap";
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string TargetCurrencySymbol { get; set; } = string.Empty;
    }
}
