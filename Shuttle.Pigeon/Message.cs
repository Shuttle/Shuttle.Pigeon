using Shuttle.Core.Contract;

namespace Shuttle.Pigeon;

public class Message(Guid id, string channel, string content, string contentType = "text/plain")
{
    private readonly List<Attachment> _attachments = [];
    private readonly List<Parameter> _parameters = [];
    private readonly List<Recipient> _recipients = [];

    public string Channel { get; } = Guard.AgainstEmpty(channel);
    public string MessageSenderName { get; private set; } = string.Empty;

    public string Content { get; private set; } = Guard.AgainstEmpty(content);
    public string ContentType { get; private set; } = Guard.AgainstEmpty(contentType);

    public bool HasSender => !string.IsNullOrWhiteSpace(Sender);

    public Guid Id { get; } = id;

    public IReadOnlyCollection<Recipient> Recipients => _recipients.AsReadOnly();
    public string Sender { get; private set; } = string.Empty;

    public string? SenderDisplayName { get; set; }

    public string Subject { get; private set; } = string.Empty;

    public Message AddAttachment(Attachment attachment)
    {
        Guard.AgainstNull(attachment);

        _attachments.RemoveAll(item => item.Name.Equals(attachment.Name, StringComparison.InvariantCultureIgnoreCase));
        _attachments.Add(attachment);

        return this;
    }

    public Message AddParameter(Parameter parameter)
    {
        Guard.AgainstNull(parameter);

        _parameters.RemoveAll(item => item.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase));
        _parameters.Add(parameter);

        return this;
    }

    public Message AddRecipient(Recipient recipient)
    {
        Guard.AgainstNull(recipient);

        _recipients.RemoveAll(item => item.Identifier.Equals(recipient.Identifier, StringComparison.InvariantCultureIgnoreCase));
        _recipients.Add(recipient);

        return this;
    }

    public Message AddRecipients(IEnumerable<Recipient> recipients)
    {
        foreach (var recipient in recipients)
        {
            AddRecipient(recipient);
        }

        return this;
    }

    public IEnumerable<Attachment> GetAttachments()
    {
        return _attachments.AsReadOnly();
    }

    public Message WithSender(string sender, string? displayName = null)
    {
        Sender = Guard.AgainstEmpty(sender);
        SenderDisplayName = displayName;

        return this;
    }
    
    public Message WithSubject(string subject)
    {
        Subject = Guard.AgainstEmpty(subject);

        return this;
    }

    public Message WithMessageSenderName(string messageSenderName)
    {
        MessageSenderName = messageSenderName;
        return this;
    }

    public Parameter? FindParameter(string name)
    {
        Guard.AgainstEmpty(name);

        return _parameters.SingleOrDefault(item => item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
    }

    public class Attachment(string name, string contentType, byte[] content)
    {
        public byte[] Content { get; } = (byte[])Guard.AgainstEmpty(content);
        public string ContentType { get; } = Guard.AgainstEmpty(contentType);

        public string Name { get; } = Guard.AgainstEmpty(name);
    }

    public class Parameter(string name, string value)
    {
        public string Name { get; } = Guard.AgainstEmpty(name);
        public string Value { get; } = Guard.AgainstEmpty(value);

        public T? GetValue<T>()
        {
            var type = typeof(T);

            var value =  (T)Convert.ChangeType(Value, Nullable.GetUnderlyingType(type) ?? type);

            if (type != typeof(DateTime))
            {
                return value;
            }

            var dateTime = (DateTime)((object?)value ?? throw new InvalidOperationException($"Parameter with name '{Name}' has value '{Value}' which cannot be converted to a DateTime object."));

            return (T)(object)DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }

    public class Recipient(string identifier, RecipientType type)
    {
        public string Identifier { get; } = Guard.AgainstEmpty(identifier);
        public RecipientType Type { get; private set; } = type;
        public string DisplayName { get; private set; } = string.Empty;
        public bool HasDisplayName => !string.IsNullOrEmpty(DisplayName);

        public Recipient WithDisplayName(string displayName)
        {
            DisplayName = Guard.AgainstEmpty(displayName);
            return this;
        }
    }
}