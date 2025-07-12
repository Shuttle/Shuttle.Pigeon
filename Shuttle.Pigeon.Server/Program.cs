using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Shuttle.Core.Contract;
using Shuttle.Core.TransactionScope;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Pigeon.Data;
using Shuttle.Pigeon.Postmark;
using Shuttle.Pigeon.SendGrid;
using Shuttle.Pigeon.Smtp;

namespace Shuttle.Pigeon.Server;

internal class Program
{
    static async Task Main(string[] args)
    {
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

        var configurationFolder = Environment.GetEnvironmentVariable("CONFIGURATION_FOLDER");

        if (string.IsNullOrEmpty(configurationFolder))
        {
            throw new ApplicationException("Environment variable `CONFIGURATION_FOLDER` has not been set.");
        }

        var appsettingsPath = Path.Combine(configurationFolder, "appsettings.json");

        if (!File.Exists(appsettingsPath))
        {
            throw new ApplicationException($"File '{appsettingsPath}' cannot be accessed/found.");
        }

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>(true)
            .AddJsonFile(appsettingsPath)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        await Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services
                    .AddTransactionScope(builder => builder.Options.Enabled = false)
                    .AddSingleton(configuration)
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddSerilog();
                    })
                    .AddServiceBus(builder =>
                    {
                        configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);
                    })
                    .AddAzureStorageQueues(builder =>
                    {
                        var queueOptions = configuration.GetSection($"{AzureStorageQueueOptions.SectionName}:Pigeon").Get<AzureStorageQueueOptions>() ?? new();

                        if (string.IsNullOrWhiteSpace(queueOptions.StorageAccount))
                        {
                            queueOptions.ConnectionString = configuration.GetConnectionString("azure") ?? string.Empty;
                        }

                        builder.AddOptions("azure", queueOptions);
                    })
                    .AddPigeon(builder =>
                    {
                        configuration.GetSection(PigeonOptions.SectionName).Bind(builder.Options);

                        builder.TryAddPostmark(configuration);
                        builder.TryAddSendGrid(configuration);
                        builder.TryAddSmtp(configuration);
                    })
                    .AddPigeonDataAccess();
            })
            .Build()
            .RunAsync();
    }
}
