﻿using ExchangeRates.Server.Models.CoinMarketCap;
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
            TargetCurrencySymbol = "EUR"
        };
        private MEO.IOptions<CoinMarketCapOptions>? _coinMarketCapOptions;
        private CoinMarketCapQuotesClient? client;

        [TestInitialize]
        public void Setup()
        {
            _coinMarketCapOptions = MEO.Options.Create(_options);
            var logger = Substitute.For<ILogger<CoinMarketCapQuotesClient>>();
            ICoinMarketCapIdMapClient mockCoinMarketCapIdMapClient = Substitute.For<ICoinMarketCapIdMapClient>();
            var mockHttpClient = Substitute.For<HttpClient>();
            client = new CoinMarketCapQuotesClient(mockCoinMarketCapIdMapClient, _coinMarketCapOptions, logger, mockHttpClient);

        }


        [TestMethod]
        public void Quote_WithSingleConvert_IsCorrectlyParsed()
        {
            var quotesJson = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExchangeRates.Server.Tests.cryptocurrency_quote_single_convert_response.json");
            using var reader = new StreamReader(quotesJson!);
            var responseString = reader.ReadToEnd();

            // Sample json uses "1" as the currency id (i.e. Bitcoin) and "EUR" as the target currency
            var result = client!.ParseCoinMarketCapQuote(responseString, "1");

            result.IfSucc(result =>
            {
                var timestamp = result?.Status?.Timestamp;
                Assert.IsNotNull(timestamp);
                Assert.AreEqual(new DateTimeOffset(2024, 12, 31, 09, 37, 03, 981, new TimeSpan(0)), timestamp);

                var price = result?.Quote?.Price;
                Assert.IsNotNull(price);
                Assert.AreEqual(90056.7922057511D, price);

                var lastUpdated = result?.Quote?.LastUpdated;
                Assert.IsNotNull(lastUpdated);
                Assert.AreEqual(new DateTimeOffset(2024, 12, 31, 09, 36, 04, 0, new TimeSpan(0)), lastUpdated);
                var currencyId = result?.CurrencyId;
                Assert.IsNotNull(currencyId);
                Assert.AreEqual("1", currencyId);
                var targetCurrencySymbol = result?.TargetCurrencySymbol;
                Assert.IsNotNull(targetCurrencySymbol);
                Assert.AreEqual("EUR", targetCurrencySymbol);
            });

            Assert.IsTrue(result.IsSuccess);

        }

        [TestMethod]
        public void Quote_WithMalformedJson_ReturnsFaulted()
        {
            string responseString = "}{";

            var result = client!.ParseCoinMarketCapQuote(responseString, "BTC");

            Assert.IsTrue(result.IsFaulted);
        }
    }
}
