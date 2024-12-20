var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ExchangeRates_Server>("exchangerates-server");

builder.Build().Run();
