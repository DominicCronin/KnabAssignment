namespace ExchangeRates.Server.Models.CoinMarketCap
{
    public record CoinMarketCapStatus
    {
        public DateTimeOffset Timestamp { get; init; }
        public int ErrorCode { get; init; }
        public string? ErrorMessage { get; init; }
        public int Elapsed { get; init; }
        public int CreditCount { get; init; }
        public string? Notice { get; init; }
    }
}
