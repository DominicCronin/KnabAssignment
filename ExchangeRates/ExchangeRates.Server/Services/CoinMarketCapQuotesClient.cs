using ExchangeRates.Server.Exceptions;
using ExchangeRates.Server.Models.CoinMarketCap;
using ExchangeRates.Server.Options;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Web;

namespace ExchangeRates.Server.Services
{
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructor not suitable for this scenario")]
    public class CoinMarketCapQuotesClient : CoinMarketCapClientBase, ICoinMarketCapQuotesClient
    {
        private readonly IOptions<CoinMarketCapOptions> _options;
        private readonly ILogger<CoinMarketCapQuotesClient> _logger;
        private readonly ICoinMarketCapIdMapClient _coinMarketCapIdMapClient;

        public CoinMarketCapQuotesClient(
            ICoinMarketCapIdMapClient coinMarketCapIdMapClient,
            IOptions<CoinMarketCapOptions> options, 
            ILogger<CoinMarketCapQuotesClient> logger) : base(options)
        {
            _options = options;
            _logger = logger;
            _coinMarketCapIdMapClient = coinMarketCapIdMapClient;
        }

        public async Task<Result<CoinMarketCapQuote>> GetLatestQuoteAsync(string symbol)
        {
            Result<string> id = await _coinMarketCapIdMapClient.GetHighestRankIdForSymbol(symbol);
            return await id.Match(
            async id =>
            {
                _logger.LogInformation("Got highest ranking id for symbol {symbol}", symbol);
                string getUriLatestQuote = BuildLatestQuoteRequestUri(id);
                var response = await GetResponseFromAPI(getUriLatestQuote);

                return response switch
                {
                    { IsSuccessStatusCode: true } when (response.Content != null) => (await ReadAndParseLatestCoinMarketCapQuote(response)),
                    { IsSuccessStatusCode: true } => new Result<CoinMarketCapQuote>(new UpstreamServiceException($"Failed to get latest listings. Status code: {response.StatusCode}")),
                    { IsSuccessStatusCode: false } => new Result<CoinMarketCapQuote>(new UpstreamServiceException($"Failed to get latest listings. Status code: {response.StatusCode}"))
                };
            },
            exception =>
            {
                return Task.FromResult(new Result<CoinMarketCapQuote>(exception));
            });
        }

        private static async Task<Result<CoinMarketCapQuote>> ReadAndParseLatestCoinMarketCapQuote(HttpResponseMessage response)
        {
            string rawLatestListings = await response.Content.ReadAsStringAsync();
            if (rawLatestListings == null)
            {
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Failed to get latest listings. Response was null."));
            }
            return ParseCoinMarketCapQuote(rawLatestListings);
        }

        internal static Result<CoinMarketCapQuote> ParseCoinMarketCapQuote(string responseString)
        {
            CoinMarketCapQuote? quote;
            try
            {
                quote = JsonSerializer.Deserialize<CoinMarketCapQuote>(responseString, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException($"Failed to parse latest quote. {ex.Message}", ex));

            }
            if (quote == null)
            {
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Failed to parse latest quote. Response was null."));
            }
            return new Result<CoinMarketCapQuote>(quote);
        }

        private string BuildLatestQuoteRequestUri(string id)
        {
            UriBuilder uriBuilder = new(_options.Value.BaseUrl);
            uriBuilder.Path += "/cryptocurrency/quotes/latest";

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["start"] = "1";
            queryString["limit"] = "1";
            queryString["id"] = id;
            queryString["convert"] = _options.Value.CurrencySymbols[0];
            uriBuilder.Query = queryString.ToString();
            return uriBuilder.ToString();
        }
    }
}
