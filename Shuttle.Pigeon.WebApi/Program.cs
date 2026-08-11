using Asp.Versioning;
using Scalar.AspNetCore;
using Serilog;
using Shuttle.Access.AspNetCore;
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
            .AddHopper(options =>
            {
                webApplicationBuilder.Configuration.GetSection(HopperOptions.SectionName).Bind(options);
            })
            .UseAzureStorageQueues(builder =>
            {
                builder.Configure("azure", options =>
                {
                    webApplicationBuilder.Configuration.GetSection($"{AzureStorageQueueOptions.SectionName}:Pigeon").Bind(options);

                    if (string.IsNullOrWhiteSpace(options.StorageAccount))
                    {
                        options.ConnectionString = webApplicationBuilder.Configuration.GetConnectionString("azure") ?? throw new ApplicationException("Missing connection string 'azure'.");
                    }
                });
            })
            .Services
            .AddPigeon(pigeonBuilder =>
            {
                pigeonBuilder
                    .UseSqlServer(builder =>
                    {
                        builder.Options.ConnectionString = webApplicationBuilder.Configuration.GetConnectionString("Pigeon") ?? throw new ApplicationException("Missing connection string 'Pigeon'.");
                    });
            })
            .AddAccessAuthorization(options =>
            {
                webApplicationBuilder.Configuration.GetSection(AccessAuthorizationOptions.SectionName).Bind(options);
            })
            .Services
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