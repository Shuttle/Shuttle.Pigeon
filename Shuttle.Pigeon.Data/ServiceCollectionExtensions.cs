using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPigeonDataAccess(this IServiceCollection services, Action<PigeonDataBuilder>? builder = null)
    {
        var pigeonDataBuilder = new PigeonDataBuilder(Guard.AgainstNull(services));

        builder?.Invoke(pigeonDataBuilder);

        services.TryAddSingleton<IValidateOptions<PigeonDataOptions>, PigeonDataOptionsValidator>();

        services.AddOptions<PigeonDataOptions>().Configure(options =>
        {
            options.ConnectionStringName = pigeonDataBuilder.Options.ConnectionStringName;
            options.MigrationsHistoryTableName = pigeonDataBuilder.Options.MigrationsHistoryTableName;
            options.CommandTimeout = pigeonDataBuilder.Options.CommandTimeout;
        });

        services.AddDbContextFactory<PigeonDbContext>((provider, dbContextOptionsBuilder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();

            var connectionString = configuration.GetConnectionString(pigeonDataBuilder.Options.ConnectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"Could not find a connection string named '{pigeonDataBuilder.Options.ConnectionStringName}'.");
            }

            dbContextOptionsBuilder.UseSqlServer(connectionString, sqlServerOptions => { sqlServerOptions.CommandTimeout(pigeonDataBuilder.Options.CommandTimeout); });
        });

        return services;
    }
}