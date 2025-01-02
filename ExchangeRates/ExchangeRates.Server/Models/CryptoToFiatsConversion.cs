namespace ExchangeRates.Server.Models
{
    public record CryptoToFiatsConversion
    {
        public required string CryptoCurrencySymbol { get; init; }
        public List<FiatRate> FiatConversions{ get; } = [];

    }

    public record FiatRate(string FiatSymbol, double Rate);
}
