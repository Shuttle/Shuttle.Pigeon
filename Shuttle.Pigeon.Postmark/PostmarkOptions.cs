namespace Shuttle.Pigeon.Postmark;

public class PostmarkOptions
{
    public const string SectionName = "Shuttle:Pigeon:Postmark";

    public string ServerToken { get; set; } = null!;
}