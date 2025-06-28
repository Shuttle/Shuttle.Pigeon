using Microsoft.Extensions.Options;
using PostmarkDotNet;

namespace Shuttle.Pigeon.Postmark;

public class PostmarkMessageSender : IMessageSender
{
    private readonly PostmarkClient _client;

    public PostmarkMessageSender(IOptions<PostmarkOptions> postmarkOptions)
    {
        if (postmarkOptions == null)
        {
            throw new ArgumentNullException(nameof(postmarkOptions));
        }

        _client = new(postmarkOptions.Value.ServerToken);
    }

    public string Channel => "email";

    public async Task SendAsync(Message message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        var postmarkMessage = new PostmarkMessage()
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
            postmarkMessage.HtmlBody = message.Content;
        }
        else
        {
            postmarkMessage.TextBody = message.Content;
        }

        if (message.HasSender)
        {
            postmarkMessage.From = message.Sender;
        }

        foreach (var attachment in message.GetAttachments())
        {
            postmarkMessage.AddAttachment(attachment.Content, attachment.Name, attachment.ContentType);
        }

        var sendResult = await _client.SendMessageAsync(postmarkMessage);

        if (sendResult.Status != PostmarkStatus.Success)
        {
            throw new($"Failed to send email: {sendResult.Message}");
        }
    }
}