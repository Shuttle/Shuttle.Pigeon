namespace Shuttle.Pigeon.Messages.v1;

public class SendMessage
{
    public Guid Id { get; set; }
    public string Channel { get; set; } = string.Empty;
    public List<Recipient> Recipients { get; set; } = [];
    public string Sender { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public List<Parameter> Parameters { get; set; } = [];

    public class Parameter
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}