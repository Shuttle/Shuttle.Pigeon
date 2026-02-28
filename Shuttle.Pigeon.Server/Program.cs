using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Shuttle.Core.Pipelines;
using Shuttle.Core.Reflection;
using Shuttle.Core.TransactionScope;
using Shuttle.Hopper;
using Shuttle.Hopper.AzureStorageQueues;
using Shuttle.Pigeon.MailKit;
using Shuttle.Pigeon.Postmark;
using Shuttle.Pigeon.SendGrid;
using Shuttle.Pigeon.SqlServer;

namespace Shuttle.Pigeon.Server;

internal class Program
{
    static async Task Main()
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
                    .AddTransactionScope(builder => builder.Configure(options =>
                    {
                        options.Enabled = false;
                    }))
                    .AddSingleton(configuration)
                    .AddLogging(loggingBuilder =>
                    {
                        loggingBuilder.ClearProviders();
                        loggingBuilder.AddSerilog();
                    })
                    .AddPipelines(pipelineBuilder =>
                    {
                        pipelineBuilder.Configure(options =>
                        {
                            options.PipelineFailed += (eventArgs, _) =>
                            {
                                Log.Error(eventArgs.Pipeline.Exception?.AllMessages() ?? string.Empty);
                                return Task.CompletedTask;
                            };

                            options.PipelineRecursiveException += (eventArgs, _) =>
                            {
                                Log.Error(eventArgs.Pipeline.Exception?.AllMessages() ?? string.Empty);
                                return Task.CompletedTask;
                            };
                        });
                    })
                    .AddHopper(hopperBuilder =>
                    {
                        configuration.GetSection(HopperOptions.SectionName).Bind(hopperBuilder.Options);

                        hopperBuilder
                            .UseAzureStorageQueues(builder =>
                            {
                                var queueOptions = configuration.GetSection($"{AzureStorageQueueOptions.SectionName}:Pigeon").Get<AzureStorageQueueOptions>() ?? new();

                                if (string.IsNullOrWhiteSpace(queueOptions.StorageAccount))
                                {
                                    queueOptions.ConnectionString = configuration.GetConnectionString("azure") ?? throw new ApplicationException("Missing connection string 'azure'.");
                                }

                                builder.AddOptions("azure", queueOptions);
                            });
                    })
                    .AddPigeon(pigeonBuilder =>
                    {
                        configuration.GetSection(PigeonOptions.SectionName).Bind(pigeonBuilder.Options);

                        pigeonBuilder.TryAddMailKit(configuration);
                        pigeonBuilder.TryAddPostmark(configuration);
                        pigeonBuilder.TryAddSendGrid(configuration);

                        pigeonBuilder
                            .UseSqlServer(builder =>
                            {
                                builder.Options.ConnectionString = configuration.GetConnectionString("Pigeon") ?? "Missing connection string 'Pigeon'.";
                            });
                    });
            })
            .Build()
            .RunAsync();
    }
}
