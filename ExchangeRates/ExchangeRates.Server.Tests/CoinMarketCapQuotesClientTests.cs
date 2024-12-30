using ExchangeRates.Server.Options;
using ExchangeRates.Server.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Reflection;
using MEO = Microsoft.Extensions.Options;

namespace ExchangeRates.Server.Tests
{
    [TestClass]
    public sealed class CoinMarketCapQuotesClientTests
    {
        private readonly CoinMarketCapOptions _options = new()
        {
            ApiKey = "Foo",
            BaseUrl = "https://example.com",
            CurrencySymbols = ["USD", "EUR"]
        };
        private MEO.IOptions<CoinMarketCapOptions>? _coinMarketCapOptions;
        private CoinMarketCapQuotesClient? client;

        [TestInitialize]
        public void Setup()
        {
            _coinMarketCapOptions = MEO.Options.Create(_options);
            var logger = Substitute.For<ILogger<CoinMarketCapQuotesClient>>();
            ICoinMarketCapIdMapClient mockCoinMarketCapIdMapClient = Substitute.For<ICoinMarketCapIdMapClient>();
            client = new CoinMarketCapQuotesClient(mockCoinMarketCapIdMapClient, _coinMarketCapOptions, logger);

        }


        [TestMethod]
        public void Quote_WithSingleConvert_IsCorrectlyParsed()
        {
            var quotesJson = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExchangeRates.Server.Tests.cryptocurrency_quote_single_convert_response.json");
            using var reader = new StreamReader(quotesJson!);
            var responseString = reader.ReadToEnd();

            var result = CoinMarketCapQuotesClient.ParseCoinMarketCapQuote(responseString);

            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void Quote_WithMalformedJson_ReturnsFaulted()
        {
            string responseString = "}{";

            var result = CoinMarketCapQuotesClient.ParseCoinMarketCapQuote(responseString);

            Assert.IsTrue(result.IsFaulted);
        }
    }
}
