using ExchangeRates.Server.Exceptions;
using ExchangeRates.Server.Models.CoinMarketCap;
using ExchangeRates.Server.Options;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Web;

namespace ExchangeRates.Server.Services
{
    public class CoinMarketCapIdMapClient(IOptions<CoinMarketCapOptions> options, ILogger<CoinMarketCapIdMapClient> logger, HttpClient httpClient) 
        : CoinMarketCapClientBase, ICoinMarketCapIdMapClient
    {

        public async Task<Result<string>> GetHighestRankIdForSymbolAsync(string symbol, CancellationToken cancellationToken)
        {
            string getHighestRankIdUri = BuildHighestRankingIdForSymbolUri(symbol);
            logger.LogInformation("Getting highest ranking id for symbol {symbol}", symbol);            
            HttpResponseMessage response = await httpClient.GetAsync(getHighestRankIdUri, cancellationToken);
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return HandleErrorResponse(responseString);
            }

            Result<CoinMarketCapIdMap> modelResult = ParseIdMapResponse(responseString);

            return modelResult.Match(
                result => {
                    return result.Data[0].Id.ToString();
                },
                exception => new Result<string>(exception)
             );            
        }

        private Result<string> HandleErrorResponse(string responseString)
        {

            CoinMarketCapError? error;
            try
            {
                error = JsonSerializer.Deserialize<CoinMarketCapError>(responseString, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                return new Result<string>(new UpstreamServiceException("Failed to parse error response from upstream service", ex));
            }

            if (error == null)
            {
                return new Result<string>(new UpstreamServiceException("Failed to parse error response from upstream service. Null returned"));
            }

            if (error.Status.ErrorCode == StatusCodes.Status400BadRequest)
            {
                return new Result<string>(new UpstreamBadRequestException($"Error from cryptocurrency service: {error.Status.ErrorMessage}"));
            }

            return new Result<string>(new UpstreamServiceException("Failed to parse error response from upstream service"));
        }

        internal static Result<CoinMarketCapIdMap> ParseIdMapResponse(string responseString)
        {
            CoinMarketCapIdMap? model;
            try
            {
                model = JsonSerializer.Deserialize<CoinMarketCapIdMap>(responseString, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                return new Result<CoinMarketCapIdMap>(new UpstreamServiceException("Failed to parse response from upstream service", ex));
            }

            if (model == null) { 
                return new Result<CoinMarketCapIdMap>(new UpstreamServiceException("Failed to parse response from upstream service. Null returned")); 
            }
            return new Result<CoinMarketCapIdMap>(model);
        }

        private string BuildHighestRankingIdForSymbolUri(string symbol)
        {
            UriBuilder uriBuilder = new(options.Value.BaseUrl);
            // /v1/cryptocurrency/map?start=1&limit=1&sort=cmc_rank&symbol=BTC
            uriBuilder.Path += "v1/cryptocurrency/map";
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["symbol"] = symbol;
            queryString["start"] = "1";
            queryString["limit"] = "1";
            queryString["sort"] = "cmc_rank";
            uriBuilder.Query = queryString.ToString();
            return uriBuilder.ToString();
        }
    }
}
