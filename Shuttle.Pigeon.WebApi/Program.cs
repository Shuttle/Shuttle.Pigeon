using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Serilog;
using Shuttle.Access.AspNetCore;
using Shuttle.Access.RestClient;
using Shuttle.Esb;
using Shuttle.Esb.AzureStorageQueues;
using Shuttle.Pigeon.Data;

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
            .AddSwaggerGen(options =>
            {
                options.SchemaGeneratorOptions.SchemaIdSelector = type => type.FullName;
                options.AddSecurityDefinition("Bearer", new()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer {jwt}\""
                });
                options.AddSecurityRequirement(new()
                {
                    {
                        new()
                        {
                            Reference = new()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        []
                    }
                });
            })
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog();
            })
            .AddServiceBus(builder =>
            {
                webApplicationBuilder.Configuration.GetSection(ServiceBusOptions.SectionName).Bind(builder.Options);
            })
            .AddAzureStorageQueues(builder =>
            {
                var queueOptions = webApplicationBuilder.Configuration.GetSection($"{AzureStorageQueueOptions.SectionName}:Pigeon").Get<AzureStorageQueueOptions>() ?? new();

                if (string.IsNullOrWhiteSpace(queueOptions.StorageAccount))
                {
                    queueOptions.ConnectionString = webApplicationBuilder.Configuration.GetConnectionString("azure") ?? string.Empty;
                }

                builder.AddOptions("azure", queueOptions);
            })
            .AddPigeonDataAccess()
            .AddAccessAuthorization(builder =>
            {
                webApplicationBuilder.Configuration.GetSection(AccessAuthorizationOptions.SectionName).Bind(builder.Options);
            })
            .AddAccessClient(builder =>
            {
                webApplicationBuilder.Configuration.GetSection(AccessClientOptions.SectionName).Bind(builder.Options);
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

        webApplicationBuilder.Services.ConfigureSwaggerGen(options => options.CustomSchemaIds(type => (type.FullName ?? string.Empty).Replace("+", "_")));

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
            .UseSwagger()
            .UseSwaggerUI()
            .UseAccessAuthorization();

        app
            .MapServerEndpoints(versionSet)
            .MapMessageEndpoints(versionSet);

        await app.RunAsync();
    }
}