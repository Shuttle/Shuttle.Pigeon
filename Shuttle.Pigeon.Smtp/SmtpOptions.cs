namespace Shuttle.Pigeon.Smtp;

public class SmtpOptions
{
    public const string SectionName = "Shuttle:Pigeon:Smtp";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SenderAddress { get; set; } = string.Empty;
    public string SenderDisplayName { get; set; } = string.Empty;
}