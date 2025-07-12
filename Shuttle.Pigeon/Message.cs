using Shuttle.Core.Contract;

namespace Shuttle.Pigeon;

public class Message
{
    private readonly List<Attachment> _attachments = [];
    private readonly List<Parameter> _parameters = [];
    private readonly List<Recipient> _recipients = [];

    public Message(Guid id, string channel, string content, string contentType = "text/plain")
    {
        Id = id;
        Channel = Guard.AgainstEmpty(channel);
        Content = Guard.AgainstEmpty(content);
        ContentType = Guard.AgainstEmpty(contentType);
    }

    public string Channel { get; }
    public string MessageSenderName { get; private set; } = string.Empty;

    public string Content { get; private set; }
    public string ContentType { get; private set; }

    public bool HasSender => !string.IsNullOrWhiteSpace(Sender);

    public Guid Id { get; }

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

    public class Attachment
    {
        public Attachment(string name, string contentType, byte[] content)
        {
            Name = Guard.AgainstEmpty(name);
            ContentType = Guard.AgainstEmpty(contentType);
            Content = (byte[])Guard.AgainstEmpty(content);
        }

        public byte[] Content { get; }
        public string ContentType { get; }

        public string Name { get; }
    }

    public class Parameter
    {
        public Parameter(string name, string value)
        {
            Name = Guard.AgainstEmpty(name);
            Value = Guard.AgainstEmpty(value);
        }

        public string Name { get; }
        public string Value { get; }

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

    public class Recipient
    {
        public Recipient(string identifier, RecipientType type)
        {
            Identifier = !string.IsNullOrWhiteSpace(identifier) ? identifier : throw new ArgumentNullException(nameof(identifier));
            Type = type;
        }

        public string Identifier { get; }
        public RecipientType Type { get; private set; }
    }
}