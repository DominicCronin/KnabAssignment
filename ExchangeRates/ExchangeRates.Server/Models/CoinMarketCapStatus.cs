namespace ExchangeRates.Server.Models
{
    public class CoinMarketCapStatus
    {
        public DateTimeOffset Timestamp { get; set; }
        public int ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public int Elapsed { get; set; }
        public int CreditCount { get; set; }
        public string? Notice { get; set; }
    }
}
