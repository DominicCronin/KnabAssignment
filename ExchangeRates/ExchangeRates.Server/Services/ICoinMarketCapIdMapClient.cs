using LanguageExt.Common;

namespace ExchangeRates.Server.Services
{
    public interface ICoinMarketCapIdMapClient
    {
        Task<Result<string>> GetHighestRankIdForSymbolAsync(string symbol, CancellationToken cancellationToken);
    }
}