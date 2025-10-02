using Microsoft.Extensions.Options;
using PostmarkDotNet;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Postmark;

public class PostmarkMessageSender(IOptions<PostmarkOptions> postmarkOptions) : IMessageSender
{
    private readonly PostmarkClient _client = new(Guard.AgainstNull(Guard.AgainstNull(postmarkOptions).Value).ServerToken);

    public string Channel => "email";
    public string Name => "postmark";

    public async Task SendAsync(Message message, CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(message);

        var msg = new PostmarkMessage()
        {
            To = string.Join(',', message.Recipients.Where(item => item.Type == RecipientType.To).Select(item => item.Identifier)),
            Cc = string.Join(',', message.Recipients.Where(item => item.Type == RecipientType.Cc).Select(item => item.Identifier)),
            Bcc = string.Join(',', message.Recipients.Where(item => item.Type == RecipientType.Bcc).Select(item => item.Identifier)),
            TrackOpens = true,
            Subject = message.Subject,
            MessageStream = "broadcast"
        };

        if (message.ContentType.Equals("text/html", StringComparison.InvariantCultureIgnoreCase))
        {
            msg.HtmlBody = message.Content;
        }
        else
        {
            msg.TextBody = message.Content;
        }

        if (message.HasSender)
        {
            msg.From = message.Sender;
        }

        foreach (var attachment in message.GetAttachments())
        {
            msg.AddAttachment(attachment.Content, attachment.Name, attachment.ContentType);
        }

        var response = await _client.SendMessageAsync(msg);

        if (response.Status != PostmarkStatus.Success)
        {
            throw new($"Failed to send email: {response.Message}");
        }
    }
}