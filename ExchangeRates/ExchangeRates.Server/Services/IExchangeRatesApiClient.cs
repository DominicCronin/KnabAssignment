using ExchangeRates.Server.Models.ExchangeRatesAPI;
using LanguageExt.Common;

namespace ExchangeRates.Server.Services
{
    public interface IExchangeRatesApiClient
    {
        Task<Result<RatesModel>> GetRatesAsync(CancellationToken cancellationToken);
    }
}