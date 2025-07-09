namespace Shuttle.Pigeon.Data;

public class PigeonDataOptions
{
    public const string SectionName = "Shuttle:Pigeon:Data";

    public string ConnectionStringName { get; set; } = "Pigeon";
    public string MigrationsHistoryTableName { get; set; } = "__EFMigrationsHistory";
    public int CommandTimeout { get; set; } = 30;
}