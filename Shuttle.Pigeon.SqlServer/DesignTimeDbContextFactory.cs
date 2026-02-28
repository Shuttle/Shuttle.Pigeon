using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.SqlServer;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PigeonDbContext>
{
    public PigeonDbContext CreateDbContext(string[] args)
    {
        /*
            Right-click on `Shuttle.Access.Data` and select `Manage User Secrets`
            {
              "ConnectionStrings": {
                "Pigeon": "Server=.;Database=Pigeon;User ID=<user>;Password=<password>;Trust Server Certificate=true;"
              },
              "Shuttle": {
                "Pigeon": {
                  "Data": {
                    "ConnectionStringName": "Pigeon"
                  }
                }
              }
            }
        */
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<DesignTimeDbContextFactory>()
            .AddCommandLine(args)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<PigeonDbContext>();

        optionsBuilder
            .UseSqlServer(configuration.GetConnectionString("Pigeon"), sqlServerOptions =>
            {
                sqlServerOptions.MigrationsHistoryTable("__EFMigrationsHistory", "pigeon");
            });

        return new(Options.Create(new PigeonSqlServerOptions()), optionsBuilder.Options);
    }
}