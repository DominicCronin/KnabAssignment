using ExchangeRates.Server.Models;
using ExchangeRates.Server.Models.CoinMarketCap;
using ExchangeRates.Server.Models.ExchangeRatesAPI;
using LanguageExt.Common;

namespace ExchangeRates.Server.Services
{
    public class CryptoCurrencyConverter(ICoinMarketCapQuotesClient coinMarketCapQuotesClient, IExchangeRatesApiClient exchangeRatesApiClient, ILogger<CryptoCurrencyConverter> logger) : ICryptoCurrencyConverter
    {
        public async Task<Result<CryptoToFiatsConversion>> GetConversionAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken)
        {
            var coinMarketCapQuote = await coinMarketCapQuotesClient.GetLatestQuoteAsync(cryptoCurrencySymbol, cancellationToken);
            return await coinMarketCapQuote.Match(
                async latestQuote =>
                {
                    var exchangeRates = await exchangeRatesApiClient.GetRatesAsync(cancellationToken);
                    return exchangeRates.Match(
                        rates =>
                        {
                            CryptoToFiatsConversion conversion = MergeQuoteAndRates(cryptoCurrencySymbol, latestQuote, rates);
                            return new Result<CryptoToFiatsConversion>(conversion);
                        },
                        error =>
                        {
                            logger.LogWarning("Failed to get exchange rates");
                            return new Result<CryptoToFiatsConversion>(error);
                        }
                    );
                },
                error =>
                {
                    logger.LogWarning("Failed to get quote for {CryptoCurrency}", cryptoCurrencySymbol);
                    return Task.FromResult(new Result<CryptoToFiatsConversion>(error));
                }
            );
        }

        internal CryptoToFiatsConversion MergeQuoteAndRates(string cryptoCurrencySymbol, CoinMarketCapQuote latestQuote, RatesModel rates)
        {
            logger.LogInformation("Merging quote and rates for {CryptoCurrency}", cryptoCurrencySymbol);
            CryptoToFiatsConversion conversion = new() { CryptoCurrencySymbol = cryptoCurrencySymbol };
            if (rates.Base != latestQuote.TargetCurrencySymbol)
            {
                throw new InvalidOperationException("Base currency does not match the target currency symbol");
            }

            // First add the crypto to base currency conversion
            // If symbol is BTC and price is 90056, then the conversion is 1 BTC = 90056 EUR
            conversion.FiatConversions.Add(latestQuote.TargetCurrencySymbol, latestQuote.Quote.Price);
            // Now we can add the other currencies. 
            foreach (var rate in rates.Rates)
            {
                // Now convert from EUR to USD or whatever
                conversion.FiatConversions.Add(rate.Key, latestQuote.Quote.Price * rate.Value);
            }

            return conversion;
        }
    }
}
