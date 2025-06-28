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
        var sqlServerStorageBuilder = new PigeonDataBuilder(Guard.AgainstNull(services));

        builder?.Invoke(sqlServerStorageBuilder);

        services.TryAddSingleton<IValidateOptions<PigeonDataOptions>, PigeonDataOptionsValidator>();

        services.AddOptions<PigeonDataOptions>().Configure(options =>
        {
            options.ConnectionStringName = sqlServerStorageBuilder.Options.ConnectionStringName;
            options.Schema = sqlServerStorageBuilder.Options.Schema;
            options.MigrationsHistoryTableName = sqlServerStorageBuilder.Options.MigrationsHistoryTableName;
            options.CommandTimeout = sqlServerStorageBuilder.Options.CommandTimeout;
        });

        services.AddDbContextFactory<PigeonDbContext>((provider, dbContextOptionsBuilder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();

            var connectionString = configuration.GetConnectionString("Pigeon");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Could not find a connection string for 'Pigeon'.");
            }

            dbContextOptionsBuilder.UseSqlServer(connectionString, sqlServerOptions => { sqlServerOptions.CommandTimeout(300); });
        });

        return services;
    }
}