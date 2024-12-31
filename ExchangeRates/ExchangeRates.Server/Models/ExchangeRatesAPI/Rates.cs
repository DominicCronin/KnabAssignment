using ExchangeRates.Server.Converters;
using System.Text.Json.Serialization;

namespace ExchangeRates.Server.Models.ExchangeRatesAPI
{
    public record RatesModel
    {
        public required bool Success { get; init; }

        [JsonConverter(typeof(UnixEpochSecondsConverter))]
        public required DateTimeOffset Timestamp { get; init; }
        public required string Base { get; init; }
        public required DateOnly Date { get; init; }
        public required Dictionary<string, decimal> Rates { get; init; }
    }
}
