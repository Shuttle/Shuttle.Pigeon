namespace Shuttle.Pigeon.Data;

public class PigeonDataOptions
{
    public const string SectionName = "Shuttle:Pigeon:Data";

    public string ConnectionStringName { get; set; } = "Pigeon";
    public string Schema { get; set; } = "pigeon";
    public string MigrationsHistoryTableName { get; set; } = "__EFMigrationsHistory";
    public int CommandTimeout { get; set; } = 30;

}