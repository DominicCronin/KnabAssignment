namespace ExchangeRates.Server.Models.CoinMarketCap
{
    public record CoinMarketCapIdMap
    {
        public required CoinMarketCapStatus Status { get; init; }
        public required CoinMarketCapIdMapData[] Data { get; init; }


    }

    public record CoinMarketCapIdMapData
    {
        public int Id { get; init; }

        public int Rank { get; init; }
        public string? Name { get; init; }

        public string? Symbol { get; init; }
        public string? Slug { get; init; }
        public DateTimeOffset FirstHistoricalData { get; init; }
        public DateTimeOffset LastHistoricalData { get; init; }
    }
}
