
using ExchangeRates.Server.Models;
using LanguageExt.Common;

namespace ExchangeRates.Server.Services
{
    public interface ICryptoCurrencyConverter
    {
        Task<Result<CryptoToFiatsConversion>> GetConversionAsync(string cryptoCurrency, CancellationToken cancellationToken);
    }
}