
namespace ExchangeRates.Server.Services
{
    public interface ICryptoCurrencyConverter
    {
        Task<decimal> GetConversionAsync(string cryptoCurrency, CancellationToken cancellationToken);
    }
}