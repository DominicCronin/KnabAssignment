namespace ExchangeRates.Server.Services
{
    public class CryptoCurrencyConverter(ICoinMarketCapQuotesClient coinMarketCapQuotesClient, IExchangeRatesApiClient exchangeRatesApiClient, Logger<CryptoCurrencyConverter> logger) : ICryptoCurrencyConverter
    {

        public async Task<decimal> GetConversionAsync(string cryptoCurrencySymbol, CancellationToken cancellationToken)
        {
            var coinMarketCapQuote = await coinMarketCapQuotesClient.GetLatestQuoteAsync(cryptoCurrencySymbol, cancellationToken);
            if (coinMarketCapQuote == null)
            {
                logger.LogWarning("Failed to get quote for {CryptoCurrency}", cryptoCurrencySymbol);
                return 0;
            }
            var exchangeRates = await exchangeRatesApiClient.GetRatesAsync(cancellationToken);
            if (exchangeRates == null)
            {
                logger.LogWarning("Failed to get exchange rates");
                return 0;
            }
            if (!exchangeRates.Rates.TryGetValue(currency, out var exchangeRate))
            {
                logger.LogWarning("Failed to get exchange rate for {Currency}", currency);
                return 0;
            }
            return amount * (decimal)coinMarketCapQuote.Price * exchangeRate;
        }
    }
}
