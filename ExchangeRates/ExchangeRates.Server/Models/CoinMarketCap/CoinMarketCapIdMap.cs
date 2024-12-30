namespace ExchangeRates.Server.Models.CoinMarketCap
{
    public class CoinMarketCapIdMap
    {
        public required CoinMarketCapStatus Status { get; set; }
        public required CoinMarketCapIdMapData[] Data { get; set; }


    }

    public class CoinMarketCapIdMapData
    {
        public int Id { get; set; }

        public int Rank { get; set; }
        public string? Name { get; set; }

        public string? Symbol { get; set; }
        public string? Slug { get; set; }
        public DateTimeOffset FirstHistoricalData { get; set; }
        public DateTimeOffset LastHistoricalData { get; set; }
    }
}
