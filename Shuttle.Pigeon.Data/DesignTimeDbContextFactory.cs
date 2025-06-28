using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shuttle.Extensions.EFCore;

namespace Shuttle.Pigeon.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PigeonDbContext>
{
    public PigeonDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddCommandLine(args)
            .Build();

        var pigeonDataOptions = configuration.GetSection(PigeonDataOptions.SectionName).Get<PigeonDataOptions>()!;

        if (pigeonDataOptions == null)
        {
            throw new InvalidOperationException($"Could not find a section called '{PigeonDataOptions.SectionName}' in the configuration.");
        }

        var schemaOverride = configuration["SchemaOverride"];

        if (!string.IsNullOrWhiteSpace(schemaOverride))
        {
            Console.WriteLine(@$"[schema-override] : original schema = '{pigeonDataOptions.Schema}' / schema override = '{schemaOverride}'");

            pigeonDataOptions.Schema = schemaOverride;
        }

        var optionsBuilder = new DbContextOptionsBuilder<PigeonDbContext>();

        optionsBuilder
            .UseSqlServer(configuration.GetConnectionString(pigeonDataOptions.ConnectionStringName),
                builder => builder.MigrationsHistoryTable(pigeonDataOptions.MigrationsHistoryTableName, pigeonDataOptions.Schema));

        optionsBuilder.ReplaceService<IMigrationsAssembly, SchemaMigrationsAssembly>();

        return new(Options.Create(pigeonDataOptions), optionsBuilder.Options);
    }
}