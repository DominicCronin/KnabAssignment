using ExchangeRates.Server.Models.CoinMarketCap;
using LanguageExt.Common;

namespace ExchangeRates.Server.Services
{
    public interface ICoinMarketCapQuotesClient
    {
        Task<Result<CoinMarketCapQuote>> GetLatestQuoteAsync(string symbol);
    }
}