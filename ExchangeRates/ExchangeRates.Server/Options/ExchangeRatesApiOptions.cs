namespace ExchangeRates.Server.Options
{
    public class ExchangeRatesApiOptions
    {
        public const string Section = "ExchangeRatesApi";
        public string ApiKey { get; set; } = String.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string[] CurrencySymbols { get; set; } = [];
    }
}
