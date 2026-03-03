using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.SqlServer;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PigeonDbContext>
{
    public PigeonDbContext CreateDbContext(string[] args)
    {
        var connectionString = GetConnectionString(args);

        var optionsBuilder = new DbContextOptionsBuilder<PigeonDbContext>();

        optionsBuilder.UseSqlServer(connectionString, sqlServerOptions =>
        {
            sqlServerOptions.MigrationsHistoryTable("__EFMigrationsHistory", "pigeon");
        });

        return new PigeonDbContext(
            Options.Create(new PigeonSqlServerOptions
            {
                ConnectionString = connectionString
            }),
            optionsBuilder.Options);
    }

    private static string GetConnectionString(string[] args)
    {
        for (var i = 0; i < args.Length; i++)
        {
            if (!string.Equals(args[i], "--connection", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return i + 1 >= args.Length 
                ? throw new ArgumentException("Missing value for --connection.") 
                : args[i + 1];
        }

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<DesignTimeDbContextFactory>()
            .AddCommandLine(args)
            .Build();

        return configuration.GetConnectionString("Pigeon")
               ?? throw new ApplicationException(
                   "Missing connection string 'Pigeon' (either via --connection or configuration).");
    }
}