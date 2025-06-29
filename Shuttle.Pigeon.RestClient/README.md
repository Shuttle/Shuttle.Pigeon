# Shuttle.Pigeon.RestClient

```
PM> Install-Package Shuttle.Pigeon.RestClient
```

## Configuration

```c#
var configuration = 
    new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

serviceCollection.AddPigeonClient(builder =>
{
    builder.Options.BaseAddress = "__BaseAddress__";
    
    // or bind from configuration
    configuration
        .GetSection(MessageClientOptions.SectionName)
        .Bind(builder.Options);

    // HttpRequest configuration
    TBD
})
```

The default JSON settings structure is as follows:

```json
{
  "Shuttle": {
    "Pigeon": {
      "Client": {
        "BaseAddress": "__BaseAddress__",
      }
    }
  }
}
```