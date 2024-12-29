using ExchangeRates.Server.Exceptions;
using ExchangeRates.Server.Models;
using ExchangeRates.Server.Options;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Web;

namespace ExchangeRates.Server.Services
{
    public class CoinMarketCapClient(IOptions<CoinMarketCapOptions> options, ILogger<CoinMarketCapClient> logger)
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
        internal async Task<Result<CoinMarketCapQuote>> GetLatestQuoteAsync(string symbol)
        {
            Result<string> id = await GetHighestRankIdForSymbol(symbol);
            return await id.Match(
            async id =>
            {
                logger.LogInformation("Got highest ranking id for symbol {symbol}", symbol);
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

        private async Task<HttpResponseMessage> GetResponseFromAPI(string uri)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.Value.ApiKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return await client.GetAsync(uri);
        }

        internal async Task<Result<string>> GetHighestRankIdForSymbol(string symbol)
        {
            string getHighestRankIdUri = BuildHighestRankingIdForSymbolUri(symbol);
            HttpResponseMessage response = await GetResponseFromAPI(getHighestRankIdUri);
            string responseString = await response.Content.ReadAsStringAsync();
            IdMapResponse? model = ParseIdMapResponse(responseString);

            return response switch
            {
                { IsSuccessStatusCode: true } when model is not null && model.Data.Length > 0 => model.Data[0].Id.ToString(),
                { IsSuccessStatusCode: true } => new Result<string>(new InvalidOperationException($"Status was success. Expected mapping data was not present.")),
                { IsSuccessStatusCode: false } when (model is not null) && (model.Status.ErrorCode == StatusCodes.Status400BadRequest)
                                                => new Result<string>(new UpstreamBadRequestException($"Error from cryptocurrency service: {model.Status.ErrorMessage}")),
                { IsSuccessStatusCode: false } => new Result<string>(new UpstreamServiceException($"Failed to get highest ranking id for symbol.", (int)response.StatusCode)),
                _ => new Result<string>(new InvalidOperationException($"Unexpected response while Getting HighestRankIdForSymbol"))
            };
        }

        internal static IdMapResponse? ParseIdMapResponse(string responseString)
        {
            return JsonSerializer.Deserialize<IdMapResponse>(responseString, _jsonSerializerOptions);
        }

        internal static Result<CoinMarketCapQuote> ParseCoinMarketCapQuote(string responseString)
        {
            CoinMarketCapQuote? quote;
            try
            {
                quote = JsonSerializer.Deserialize<CoinMarketCapQuote>(responseString);
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

        private string BuildHighestRankingIdForSymbolUri(string symbol)
        {
            UriBuilder uriBuilder = new(options.Value.BaseUrl);
            // /v1/cryptocurrency/map?start=1&limit=1&sort=cmc_rank&symbol=BTC
            uriBuilder.Path += "/cryptocurrency/map";
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["symbol"] = symbol;
            queryString["start"] = "1";
            queryString["limit"] = "1";
            queryString["sort"] = "cmc_rank";
            uriBuilder.Query = queryString.ToString();
            return uriBuilder.ToString();
        }
        private string BuildLatestQuoteRequestUri(string id)
        {
            UriBuilder uriBuilder = new(options.Value.BaseUrl);
            uriBuilder.Path += "/cryptocurrency/quotes/latest";

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["start"] = "1";
            queryString["limit"] = "1";
            queryString["id"] = id;
            queryString["convert"] = options.Value.CurrencySymbols[0];
            uriBuilder.Query = queryString.ToString();
            return uriBuilder.ToString();
        }

    }
}
