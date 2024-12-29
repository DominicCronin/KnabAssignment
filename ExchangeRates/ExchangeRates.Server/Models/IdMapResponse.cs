namespace ExchangeRates.Server.Models
{
    public class IdMapResponse
    {
        public required CoinMarketCapStatus Status { get; set; }
        public required IdMapResponseData[] Data { get; set; }


    }

    public class IdMapResponseData
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
