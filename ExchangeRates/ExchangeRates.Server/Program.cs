using ExchangeRates.Server.Options;
using ExchangeRates.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CoinMarketCapOptions>(builder.Configuration.GetSection(CoinMarketCapOptions.Section));
builder.Services.Configure<ExchangeRatesApiOptions>(builder.Configuration.GetSection(ExchangeRatesApiOptions.Section));

builder.Services.AddScoped<ICoinMarketCapIdMapClient, CoinMarketCapIdMapClient>();
builder.Services.AddScoped<ICoinMarketCapQuotesClient, CoinMarketCapQuotesClient>();

builder.Services.AddScoped<IExchangeRatesApiClient, ExchangeRatesApiClient>();

// TODO - DRY
builder.Services.AddHttpClient<ICoinMarketCapIdMapClient, CoinMarketCapIdMapClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<CoinMarketCapOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.ApiKey);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<ICoinMarketCapQuotesClient, CoinMarketCapQuotesClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<CoinMarketCapOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", options.ApiKey);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<IExchangeRatesApiClient, ExchangeRatesApiClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ExchangeRatesApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});


builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/cmc", async (HttpContext context, [FromServices] ICoinMarketCapQuotesClient client) =>
{
    var cancellationToken = context.RequestAborted;
    LanguageExt.Common.Result<ExchangeRates.Server.Models.CoinMarketCap.CoinMarketCapQuote> result;     
    try
    {
        result = await client.GetLatestQuoteAsync("BTC", cancellationToken);
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(StatusCodes.Status499ClientClosedRequest);
    }
    return result.Match(
        success => Results.Ok(success),
        failure => Results.BadRequest(failure));
});

app.MapGet("/exc", async (HttpContext context, [FromServices] IExchangeRatesApiClient client) =>
{
    var cancellationToken = context.RequestAborted;
    LanguageExt.Common.Result<ExchangeRates.Server.Models.ExchangeRatesAPI.RatesModel> result;
    try
    {
        result = await client.GetRates(cancellationToken);
    }
    catch (OperationCanceledException)
    {
        return  Results.StatusCode(StatusCodes.Status499ClientClosedRequest);
    }

    return result.Match(
        success => Results.Ok(success),
        failure => Results.BadRequest(failure));
});

app.MapGet("/convert", async (HttpContext context, [FromServices] IExchangeRatesApiClient client) =>
{
    var cancellationToken = context.RequestAborted;
    LanguageExt.Common.Result<ExchangeRates.Server.Models.ExchangeRatesAPI.RatesModel> result;
    try
    {
        result = await client.GetRates(cancellationToken);
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(StatusCodes.Status499ClientClosedRequest);
    }

    return result.Match(
        success => Results.Ok(success),
        failure => Results.BadRequest(failure));
});


app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapFallbackToFile("/index.html");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
