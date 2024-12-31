namespace ExchangeRates.Server.Options
{
    public record ExchangeRatesApiOptions
    {
        public const string Section = "ExchangeRatesApi";
        public required string ApiKey { get; init; }
        public required string BaseUrl { get; init; }
        public required string[] TargetCurrencySymbols { get; init; }
        public required string BaseCurrency { get; init; }
    }
}
