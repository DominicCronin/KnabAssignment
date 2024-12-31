using ExchangeRates.Server.Converters;
using System.Text.Json.Serialization;

namespace ExchangeRates.Server.Models.ExchangeRatesAPI
{
    public record RatesModel
    {
        public bool Success { get; init; }

        [JsonConverter(typeof(UnixEpochSecondsConverter))]
        public DateTimeOffset Timestamp { get; init; }
        public string Base { get; set; }
        public DateOnly Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}
