# Dominic-Cronin-Knab

This project relies on API keys being available in the configuration. Please view the [ExchangeRates/ExchangeRates.Server/appsettings.json](ExchangeRates/ExchangeRates.Server/appsettings.json) file and configure your User Secrets to provide the missing keys.

To run the project, from the root of the repository use the following command: 

```
dotnet run --project .\ExchangeRates.AppHost\
```
The output from this command provides a link to the Aspire dashboard, where you can navigate to the client endpoint to see the application working. 