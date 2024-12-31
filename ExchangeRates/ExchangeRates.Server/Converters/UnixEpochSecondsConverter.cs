using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeRates.Server.Converters;
public class UnixEpochSecondsConverter : JsonConverter<DateTimeOffset>
    {
    // Thanks ChatGPT!
    // Consciously ignoring YAGNI here, as this is a good example of a simple, reusable, and useful converter.
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException("Expected a number representing Unix epoch seconds.");
            }

            long seconds = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            long seconds = value.ToUnixTimeSeconds();
            writer.WriteNumberValue(seconds);
        }
    }

