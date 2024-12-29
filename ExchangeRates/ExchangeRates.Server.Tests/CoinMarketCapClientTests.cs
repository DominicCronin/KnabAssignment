using ExchangeRates.Server.Options;
using ExchangeRates.Server.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Reflection;
using MEO = Microsoft.Extensions.Options;

namespace ExchangeRates.Server.Tests
{
    [TestClass]
    public sealed class CoinMarketCapClientTests
    {
        private readonly CoinMarketCapOptions _options = new()
        {
            ApiKey = "Foo",
            BaseUrl = "https://example.com",
            CurrencySymbols = ["USD", "EUR"]
        };
        private MEO.IOptions<CoinMarketCapOptions>? _coinMarketCapOptions;
        private CoinMarketCapClient? client;

        [TestInitialize]
        public void Setup()
        {
            _coinMarketCapOptions = MEO.Options.Create(_options);
            var logger = Substitute.For<ILogger<CoinMarketCapClient>>();
            client = new CoinMarketCapClient(_coinMarketCapOptions, logger);

        }

        [TestMethod]
        public void IdMapResponse_IsCorrectlyParsed()
        {
            var getFirstIdForSymbolJson = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExchangeRates.Server.Tests.GetFirstIdForSymbol.json");
            using var reader = new StreamReader(getFirstIdForSymbolJson!);
            var responseString = reader.ReadToEnd();

            var result = CoinMarketCapClient.ParseIdMapResponse(responseString);
            
            Assert.AreEqual<DateTimeOffset>(DateTimeOffset.Parse("2024-12-27T10:00:16.657Z"), result!.Status.Timestamp);
            Assert.AreEqual(0, result.Status.ErrorCode);
            Assert.AreEqual(null, result.Status.ErrorMessage);
            Assert.AreEqual(13, result.Status.Elapsed);
            Assert.AreEqual(1, result.Status.CreditCount);
            Assert.AreEqual(null, result.Status.Notice);
            Assert.AreEqual(1, result.Data[0]!.Id);
            Assert.AreEqual(1, result.Data[0]!.Rank);
            Assert.AreEqual("Bitcoin", result.Data[0]!.Name);
            Assert.AreEqual("bitcoin", result.Data[0]!.Slug);
            Assert.AreEqual<DateTimeOffset>(DateTimeOffset.Parse("2010-07-13T00:05:00.000Z"), result.Data[0]!.FirstHistoricalData);
            Assert.AreEqual<DateTimeOffset>(DateTimeOffset.Parse("2024-12-27T09:45:00.000Z"), result.Data[0]!.LastHistoricalData);

            Assert.AreEqual(5, result.Data.Length);
        }

    }
}
