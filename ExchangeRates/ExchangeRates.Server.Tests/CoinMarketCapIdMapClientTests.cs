﻿using ExchangeRates.Server.Options;
using ExchangeRates.Server.Services;
using LanguageExt.Pipes;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Reflection;
using MEO = Microsoft.Extensions.Options;

namespace ExchangeRates.Server.Tests
{
    [TestClass]
    public sealed class CoinMarketCapIdMapClientTests
    {

        [TestMethod]
        public void IdMapResponse_IsCorrectlyParsed()
        {
            var getFirstIdForSymbolJson = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExchangeRates.Server.Tests.GetFirstIdForSymbol.json");
            using var reader = new StreamReader(getFirstIdForSymbolJson!);
            var responseString = reader.ReadToEnd();

            var result = CoinMarketCapIdMapClient.ParseIdMapResponse(responseString);

            result.IfSucc(result =>
            {
                    Assert.AreEqual(DateTimeOffset.Parse("2024-12-27T10:00:16.657Z"), result.Status.Timestamp);
                    Assert.AreEqual(0, result.Status.ErrorCode);
                    Assert.AreEqual(null, result.Status.ErrorMessage);
                    Assert.AreEqual(13, result.Status.Elapsed);
                    Assert.AreEqual(1, result.Status.CreditCount);
                    Assert.AreEqual(null, result.Status.Notice);
                    Assert.AreEqual(1, result.Data[0]!.Id);
                    Assert.AreEqual(1, result.Data[0]!.Rank);
                    Assert.AreEqual("Bitcoin", result.Data[0]!.Name);
                    Assert.AreEqual("bitcoin", result.Data[0]!.Slug);
                    Assert.AreEqual(DateTimeOffset.Parse("2010-07-13T00:05:00.000Z"), result.Data[0]!.FirstHistoricalData);
                    Assert.AreEqual(DateTimeOffset.Parse("2024-12-27T09:45:00.000Z"), result.Data[0]!.LastHistoricalData);
                    Assert.AreEqual(6, result.Data.Length);
                }
            );

            Assert.IsTrue(result.IsSuccess);
        }
    }
}