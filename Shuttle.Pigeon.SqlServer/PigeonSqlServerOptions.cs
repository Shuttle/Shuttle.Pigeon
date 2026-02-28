namespace Shuttle.Pigeon.SqlServer;

public class PigeonSqlServerOptions
{
    public const string SectionName = "Shuttle:Pigeon:SqlServer";

    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
}