using ExchangeRates.Server.Exceptions;
using ExchangeRates.Server.Models.CoinMarketCap;
using ExchangeRates.Server.Options;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace ExchangeRates.Server.Services
{
    [SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Primary constructor not suitable for this scenario")]
    public class CoinMarketCapQuotesClient : CoinMarketCapClientBase, ICoinMarketCapQuotesClient
    {
        private readonly CoinMarketCapOptions _options;
        private readonly ILogger<CoinMarketCapQuotesClient> _logger;
        private readonly ICoinMarketCapIdMapClient _coinMarketCapIdMapClient;

        public CoinMarketCapQuotesClient(
            ICoinMarketCapIdMapClient coinMarketCapIdMapClient,
            IOptions<CoinMarketCapOptions> options, 
            ILogger<CoinMarketCapQuotesClient> logger) : base(options)
        {
            _options = options.Value;
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
                    { IsSuccessStatusCode: true } when (response.Content != null) => (await ReadAndParseLatestCoinMarketCapQuote(response, id)),
                    { IsSuccessStatusCode: true } => new Result<CoinMarketCapQuote>(new UpstreamServiceException($"Failed to get id for symbol. Content was null. Status code: {response.StatusCode}")),
                    { IsSuccessStatusCode: false } => new Result<CoinMarketCapQuote>(new UpstreamServiceException($"Failed to get id for symbol. Status code: {response.StatusCode}"))
                };
            },
            exception =>
            {
                return Task.FromResult(new Result<CoinMarketCapQuote>(exception));
            });
        }

        private async Task<Result<CoinMarketCapQuote>> ReadAndParseLatestCoinMarketCapQuote(HttpResponseMessage response, string id)
        {
            string rawLatestQuote = await response.Content.ReadAsStringAsync();
            if (rawLatestQuote == null)
            {
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Failed to get latest quote. Response was null."));
            }
            return ParseCoinMarketCapQuote(rawLatestQuote, id);
        }

        internal Result<CoinMarketCapQuote> ParseCoinMarketCapQuote(string responseString, string id)
        {
            if (responseString == null){ return new Result<CoinMarketCapQuote>(new ArgumentNullException(responseString));}
            if (id == null) { return new Result<CoinMarketCapQuote>(new ArgumentNullException(id));}

            CoinMarketCapQuote quote = new();
            JsonNode? responseDom;
            try
            {
                responseDom = JsonNode.Parse(responseString);
            }
            catch (JsonException ex)
            {
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Unable to parse response to dom", ex));
            }

            if (responseDom == null)
            {
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Response DOM was null"));
            }

            JsonNode? statusNode = responseDom["status"];
            if (statusNode == null)
            {
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Status element was null"));
            }

            JsonNode? quoteNode = responseDom["data"]?[id]?["quote"]?[_options.TargetCurrencySymbol];                           
            if (quoteNode == null)
            {
                _logger.LogError($"Quote element was null when querying data.id.quote.{_options.TargetCurrencySymbol}");
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Quote element was null"));
            }

            try
            {
                quote.Status = JsonSerializer.Deserialize<CoinMarketCapStatus>(statusNode.ToString(), _jsonSerializerOptions);
                quote.Quote = JsonSerializer.Deserialize<CoinMarketCapTargetCurrencyQuote>(quoteNode.ToString(), _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Unable to parse response to models", ex));
            }
            quote.CurrencyId = id;
            quote.TargetCurrencySymbol = _options.TargetCurrencySymbol;

            return new Result<CoinMarketCapQuote>(quote);
        }

        private string BuildLatestQuoteRequestUri(string id)
        {
            UriBuilder uriBuilder = new(_options.BaseUrl);
            uriBuilder.Path += "v2/cryptocurrency/quotes/latest";

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["id"] = id;
            queryString["convert"] = _options.TargetCurrencySymbol;
            uriBuilder.Query = queryString.ToString();
            return uriBuilder.ToString();
        }
    }
}
