namespace Shuttle.Pigeon.Tests;

public class MailOptions
{
    public const string SectionName = "Fixtures:Mail";

    public string Recipient { get; set; } = null!;
    public string Sender { get; set; } = null!;
}