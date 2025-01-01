using ExchangeRates.Server.Models.ExchangeRatesAPI;
using Google.Protobuf.WellKnownTypes;
using System.Text.Json;

namespace ExchangeRates.Server.Tests;

[TestClass]
public class RatesModelTests
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    [TestMethod]
    public void Deserialization_fills_fields_correctly()
    {
        string inputJson = @"
        {
            ""success"": true,
            ""timestamp"": 1735640944,
            ""base"": ""EUR"",
            ""date"": ""2024-12-31"",
            ""rates"": {
                ""USD"": 1.041178,
                ""BRL"": 6.438957,
                ""GBP"": 0.829288,
                ""AUD"": 1.674553
            }
        }
        ";

        var ratesModel = JsonSerializer.Deserialize<RatesModel>(inputJson, _jsonSerializerOptions);

        Assert.IsNotNull(ratesModel);
        Assert.AreEqual(true, ratesModel.Success);
        Assert.AreEqual(DateTimeOffset.FromUnixTimeSeconds(1735640944), ratesModel.Timestamp);
        Assert.AreEqual("EUR", ratesModel.Base);
        Assert.AreEqual(new DateOnly(2024, 12, 31), ratesModel.Date);
        Assert.AreEqual(4, ratesModel.Rates.Count);
        Assert.AreEqual(1.041178D, ratesModel.Rates["USD"]);
        Assert.AreEqual(6.438957D, ratesModel.Rates["BRL"]);
        Assert.AreEqual(0.829288D, ratesModel.Rates["GBP"]);
        Assert.AreEqual(1.674553D, ratesModel.Rates["AUD"]);
    }
}
