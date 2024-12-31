namespace ExchangeRates.Server.Options
{
    public record CoinMarketCapOptions
    {
        public const string Section = "CoinMarketCap";
        public required string ApiKey { get; init; }
        public required string BaseUrl { get; init; }
        public required string TargetCurrencySymbol { get; init; }
    }
}
