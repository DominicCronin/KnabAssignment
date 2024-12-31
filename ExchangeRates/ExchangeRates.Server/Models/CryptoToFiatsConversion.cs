namespace ExchangeRates.Server.Models
{
    public class CryptoToFiatsConversion
    {
        public required string CryptoCurrencySymbol { get; init; }
        public Dictionary<string, double> FiatConversions{ get; } = [];

    }
}
