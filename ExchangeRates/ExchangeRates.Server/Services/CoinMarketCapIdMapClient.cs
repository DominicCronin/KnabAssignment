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
    public class CoinMarketCapIdMapClient : CoinMarketCapClientBase, ICoinMarketCapIdMapClient
    {
        private readonly IOptions<CoinMarketCapOptions> _options;
        private readonly ILogger<CoinMarketCapIdMapClient> _logger;

        public CoinMarketCapIdMapClient(IOptions<CoinMarketCapOptions> options, ILogger<CoinMarketCapIdMapClient> logger) : base(options)
        {
            _options = options;
            _logger = logger;
        }

        public async Task<Result<string>> GetHighestRankIdForSymbol(string symbol)
        {
            string getHighestRankIdUri = BuildHighestRankingIdForSymbolUri(symbol);
            _logger.LogInformation("Getting highest ranking id for symbol {symbol}", symbol);
            HttpResponseMessage response = await GetResponseFromAPI(getHighestRankIdUri);
            string responseString = await response.Content.ReadAsStringAsync();
            Result<CoinMarketCapIdMap> modelResult = ParseIdMapResponse(responseString);

            var matchResult = modelResult.Match(
                result => {
                    return response switch
                    {
                        { IsSuccessStatusCode: true } when result is not null && result.Data.Length > 0 => result.Data[0].Id.ToString(),
                        { IsSuccessStatusCode: true } => new Result<string>(new InvalidOperationException($"Status was success. Expected mapping data was not present.")),
                        { IsSuccessStatusCode: false } when (result is not null) && (result.Status.ErrorCode == StatusCodes.Status400BadRequest)
                            => new Result<string>(new UpstreamBadRequestException($"Error from cryptocurrency service: {result.Status.ErrorMessage}")),
                        { IsSuccessStatusCode: false } => new Result<string>(new UpstreamServiceException($"Failed to get highest ranking id for symbol.", (int)response.StatusCode)),
                        _ => new Result<string>(new InvalidOperationException($"Unexpected response while Getting HighestRankIdForSymbol"))
                    };
                },
                    exception => new Result<string>(exception)
                );
            return matchResult;

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
            UriBuilder uriBuilder = new(_options.Value.BaseUrl);
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
    }
}
