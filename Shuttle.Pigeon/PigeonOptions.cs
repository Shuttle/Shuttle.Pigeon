namespace Shuttle.Pigeon;

public class PigeonOptions
{
    public const string SectionName = "Shuttle:Pigeon";

    public bool ValidateServerCertificate { get; set; } = true;
    public List<ChannelDefaultMessageSender> ChannelDefaultMessageSenders { get; set; } = [];
}

public class ChannelDefaultMessageSender
{
    public string Channel { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}