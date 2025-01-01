namespace ExchangeRates.Server.Models.CoinMarketCap
{
    public record CoinMarketCapError
    {
        public required CoinMarketCapStatus Status { get; init; }
    }
}
