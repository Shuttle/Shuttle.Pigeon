using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.SqlServer;

public static class PigeonBuilderExtensions
{
    public static PigeonBuilder UseSqlServer(this PigeonBuilder pigeonBuilder, Action<PigeonSqlServerBuilder>? builder = null)
    {
        var services = Guard.AgainstNull(pigeonBuilder.Services);
        var pigeonDataBuilder = new PigeonSqlServerBuilder(services);

        builder?.Invoke(pigeonDataBuilder);

        services.TryAddSingleton<IValidateOptions<PigeonSqlServerOptions>, PigeonSqlServerOptionsValidator>();

        services.AddOptions<PigeonSqlServerOptions>().Configure(options =>
        {
            options.ConnectionString = pigeonDataBuilder.Options.ConnectionString;
            options.CommandTimeout = pigeonDataBuilder.Options.CommandTimeout;
        });

        services.AddDbContext<PigeonDbContext>((_, dbContextOptionsBuilder) =>
        {
            dbContextOptionsBuilder.UseSqlServer(pigeonDataBuilder.Options.ConnectionString, sqlServerOptions =>
            {
                sqlServerOptions.MigrationsHistoryTable("__EFMigrationsHistory", "pigeon");
                sqlServerOptions.CommandTimeout(pigeonDataBuilder.Options.CommandTimeout);
            });
        });

        return pigeonBuilder;
    }
}