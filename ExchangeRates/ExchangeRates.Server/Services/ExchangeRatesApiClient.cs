using ExchangeRates.Server.Exceptions;
using ExchangeRates.Server.Models.ExchangeRatesAPI;
using ExchangeRates.Server.Options;
using LanguageExt.Common;
using LanguageExt.Pipes;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace ExchangeRates.Server.Services
{
    public class ExchangeRatesApiClient(IOptions<ExchangeRatesApiOptions> options, ILogger<ExchangeRatesApiClient> logger, HttpClient client) : IExchangeRatesApiClient
    {

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        public async Task<Result<RatesModel>> GetRatesAsync(CancellationToken cancellationToken)
        {
            string ratesUri = BuildRatesUri();
            HttpResponseMessage response = await client.GetAsync(ratesUri, cancellationToken);
            string responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return response switch
            {
                { IsSuccessStatusCode: true } => ParseRatesResponse(responseString),
                { IsSuccessStatusCode: false } => new Result<RatesModel>(new UpstreamServiceException($"Failed to get rates. Status code: {response.StatusCode}"))
            };
        }

        internal Result<RatesModel> ParseRatesResponse(string responseString)
        {
            RatesModel? model;
            try
            {
                model = JsonSerializer.Deserialize<RatesModel>(responseString, _jsonSerializerOptions);
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Failed to parse response from upstream service");
                return new Result<RatesModel>(new UpstreamServiceException("Failed to parse response from upstream service", ex));
            }
            if (model is null)
            {
                logger.LogError("Failed to parse response from upstream service. Model was null");
                return new Result<RatesModel>(new UpstreamServiceException("Failed to parse response from upstream service. Model was null"));
            }
            return new Result<RatesModel>(model);
        }

        internal string BuildRatesUri()
        {
            UriBuilder uriBuilder = new(options.Value.BaseUrl);
            // {{BaseURL}}latest?access_key={{API_KEY}}&base=EUR&symbols=USD,BRL,GBP,AUD
            uriBuilder.Path += "latest";
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["access_key"] = options.Value.ApiKey; //  Really? I mean, like, really????? YGTBKM
            queryString["base"] = options.Value.BaseCurrency;
            queryString["symbols"] = options.Value.TargetCurrencySymbols;
            uriBuilder.Query = queryString.ToString();
            string uri = uriBuilder.ToString();
            // Let's get the access key into the logging as well. Why not, eh?
            logger.LogInformation("Built URI: {uri}", uri);
            return uri;
        }
    }
}
