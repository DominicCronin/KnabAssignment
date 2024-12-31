using ExchangeRates.Server.Exceptions;
using ExchangeRates.Server.Models.CoinMarketCap;
using ExchangeRates.Server.Options;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;

namespace ExchangeRates.Server.Services
{
    public class CoinMarketCapQuotesClient(
            ICoinMarketCapIdMapClient coinMarketCapIdMapClient,
            IOptions<CoinMarketCapOptions> options,
            ILogger<CoinMarketCapQuotesClient> logger,
            HttpClient httpClient) : CoinMarketCapClientBase, ICoinMarketCapQuotesClient
    {

        public async Task<Result<CoinMarketCapQuote>> GetLatestQuoteAsync(string symbol)
        {
            Result<string> id = await coinMarketCapIdMapClient.GetHighestRankIdForSymbol(symbol);
            return await id.Match(
            async id =>
            {
                logger.LogInformation("Got highest ranking id for symbol {symbol}", symbol);
                string getUriLatestQuote = BuildLatestQuoteRequestUri(id);
                var response = await httpClient.GetAsync(getUriLatestQuote);

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

            JsonNode? quoteNode = responseDom["data"]?[id]?["quote"]?[options.Value.TargetCurrencySymbol];                           
            if (quoteNode == null)
            {
                logger.LogError("Quote element was null when querying data.id.quote.{TargetCurrencySymbol}", options.Value.TargetCurrencySymbol);
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Quote element was null"));
            }

            CoinMarketCapQuote quote;
            try
            {
                quote = new()
                {
                    CurrencyId = id,
                    TargetCurrencySymbol = options.Value.TargetCurrencySymbol,
                    Status = JsonSerializer.Deserialize<CoinMarketCapStatus>(statusNode.ToString(), _jsonSerializerOptions),
                    Quote = JsonSerializer.Deserialize<CoinMarketCapTargetCurrencyQuote>(quoteNode.ToString(), _jsonSerializerOptions)

                };
            }
            catch (JsonException ex)
            {
                return new Result<CoinMarketCapQuote>(new UpstreamServiceException("Unable to parse response to models", ex));
            }

            return new Result<CoinMarketCapQuote>(quote);
        }

        private string BuildLatestQuoteRequestUri(string id)
        {
            UriBuilder uriBuilder = new(options.Value.BaseUrl);
            uriBuilder.Path += "v2/cryptocurrency/quotes/latest";

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["id"] = id;
            queryString["convert"] = options.Value.TargetCurrencySymbol;
            uriBuilder.Query = queryString.ToString();
            return uriBuilder.ToString();
        }
    }
}
