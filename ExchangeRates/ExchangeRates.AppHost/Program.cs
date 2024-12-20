var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.ExchangeRates_Server>("exchangerates-server");
builder.AddNpmApp("client","../exchangeRates.client", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
