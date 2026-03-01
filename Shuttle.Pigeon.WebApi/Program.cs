using Asp.Versioning;
using Azure.Identity;
using Scalar.AspNetCore;
using Serilog;
using Shuttle.Access.AspNetCore;
using Shuttle.Access.RestClient;
using Shuttle.Hopper;
using Shuttle.Hopper.AzureStorageQueues;
using Shuttle.Pigeon.SqlServer;

namespace Shuttle.Pigeon.WebApi;

public class Program
{
    public static async Task Main(string[] args)
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

        var webApplicationBuilder = WebApplication.CreateBuilder(args);

        webApplicationBuilder.Configuration
            .AddUserSecrets<Program>(true)
            .AddJsonFile(appsettingsPath);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(webApplicationBuilder.Configuration)
            .CreateLogger();

        webApplicationBuilder.Services
            .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
            .AddEndpointsApiExplorer()
            .AddOpenApi(options =>
            {
                options.AddSchemaTransformer((schema, _, _) =>
                {
                    schema.Title = schema.Title?.Replace("+", "_");
                    return Task.CompletedTask;
                });
            })
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            })
            .AddHopper(hopperBuilder =>
            {
                webApplicationBuilder.Configuration.GetSection(HopperOptions.SectionName).Bind(hopperBuilder.Options);

                hopperBuilder
                    .UseAzureStorageQueues(builder =>
                    {
                        var queueOptions = webApplicationBuilder.Configuration.GetSection($"{AzureStorageQueueOptions.SectionName}:Pigeon").Get<AzureStorageQueueOptions>() ?? new();

                        if (string.IsNullOrWhiteSpace(queueOptions.StorageAccount))
                        {
                            queueOptions.ConnectionString = webApplicationBuilder.Configuration.GetConnectionString("azure") ?? throw new ApplicationException("Missing connection string 'azure'.");
                        }

                        builder.AddOptions("azure", queueOptions);
                    });
            })
            .AddPigeon(pigeonBuilder =>
            {
                pigeonBuilder
                    .UseSqlServer(builder =>
                    {
                        builder.Options.ConnectionString = webApplicationBuilder.Configuration.GetConnectionString("Pigeon") ?? "Missing connection string 'Pigeon'.";
                    });
            })
            .AddAccessClient(clientBuilder =>
            {
                webApplicationBuilder.Configuration.GetSection(AccessClientOptions.SectionName).Bind(clientBuilder.Options);

                clientBuilder.UseBearerAuthenticationProvider(providerBuilder =>
                {
                    providerBuilder.Options.GetBearerAuthenticationContextAsync = async (_, _) =>
                    {
                        var token = (await new DefaultAzureCredential().GetTokenAsync(new(["https://management.azure.com/.default"]), CancellationToken.None)).Token;

                        return new(token);
                    };
                });
            })
            .AddAccessAuthorization(builder =>
            {
                webApplicationBuilder.Configuration.GetSection(AccessAuthorizationOptions.SectionName).Bind(builder.Options);
            })
            .AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

        var apiVersion1 = new ApiVersion(1, 0);

        webApplicationBuilder.Services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = apiVersion1;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        var app = webApplicationBuilder.Build();

        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(apiVersion1)
            .ReportApiVersions()
            .Build();

        app
            .UseCors()
            .UseAccessAuthorization();

        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options
                .WithTitle("Shuttle Pigeon API")
                .WithTheme(ScalarTheme.DeepSpace)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        app
            .MapServerEndpoints(versionSet)
            .MapMessageEndpoints(versionSet);

        await app.RunAsync();
    }
}