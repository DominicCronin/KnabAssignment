var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.ExchangeRates_Server>("exchangeratesapi");

builder.AddNpmApp("client","../exchangeRates.client", "dev")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpsEndpoint(env: "VITE_PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
