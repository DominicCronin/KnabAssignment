using ExchangeRates.Server.Exceptions;
using ExchangeRates.Server.Options;
using ExchangeRates.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CoinMarketCapOptions>(builder.Configuration.GetSection(CoinMarketCapOptions.Section));
builder.Services.Configure<ExchangeRatesApiOptions>(builder.Configuration.GetSection(ExchangeRatesApiOptions.Section));

builder.Services.AddScoped<ICoinMarketCapIdMapClient, CoinMarketCapIdMapClient>();
builder.Services.AddScoped<ICoinMarketCapQuotesClient, CoinMarketCapQuotesClient>();

builder.Services.AddScoped<IExchangeRatesApiClient, ExchangeRatesApiClient>();

builder.Services.AddScoped<ICryptoCurrencyConverter, CryptoCurrencyConverter>();

Action<IServiceProvider, HttpClient>  configureCoinMarketCapClient = (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<CoinMarketCapOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.ApiKey);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
};

builder.Services.AddHttpClient<ICoinMarketCapIdMapClient, CoinMarketCapIdMapClient>(configureCoinMarketCapClient);
builder.Services.AddHttpClient<ICoinMarketCapQuotesClient, CoinMarketCapQuotesClient>(configureCoinMarketCapClient);

builder.Services.AddHttpClient<IExchangeRatesApiClient, ExchangeRatesApiClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ExchangeRatesApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


builder.AddServiceDefaults();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.MapStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/convert", async (HttpContext context, [FromServices] ICryptoCurrencyConverter client, string symbol) =>
{
    Regex validSymbol = ValidSymbol();
    if (!validSymbol.IsMatch(symbol)) { 
        return Results.BadRequest("The symbol parameter must be three capital letters (A-Z)");
    }

    var cancellationToken = context.RequestAborted;
    LanguageExt.Common.Result<ExchangeRates.Server.Models.CryptoToFiatsConversion> result;
    try
    {
        result = await client.GetConversionAsync(symbol, cancellationToken);
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(StatusCodes.Status499ClientClosedRequest);
    }

    return result.Match(
        success => Results.Ok(success),
        failure => {
            return failure switch {
                UpstreamBadRequestException ex => Results.BadRequest($"Unable to process a request to convert {symbol}"),                                
                _ => Results.InternalServerError(failure)
            };
        });
});

app.MapFallbackToFile("/index.html");

app.Run();

partial class Program
{
    [GeneratedRegex(@"^[A-Z]{3}$", RegexOptions.Compiled)]
    private static partial Regex ValidSymbol();
}