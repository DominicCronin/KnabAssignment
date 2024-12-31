using ExchangeRates.Server.Models.CoinMarketCap;
using LanguageExt.Common;
using System.Threading;

namespace ExchangeRates.Server.Services
{
    public interface ICoinMarketCapQuotesClient
    {
        Task<Result<CoinMarketCapQuote>> GetLatestQuoteAsync(string symbol, CancellationToken cancellationToken);
    }
}