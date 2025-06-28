namespace Shuttle.Pigeon.SendGrid;

public class SendGridOptions
{
    public const string SectionName = "Shuttle:Pigeon:SendGrid";

    public string ApiKey { get; set; } = null!;
}