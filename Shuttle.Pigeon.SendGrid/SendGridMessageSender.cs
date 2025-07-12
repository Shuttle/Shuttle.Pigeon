using System.Net;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.SendGrid;

public class SendGridMessageSender : IMessageSender
{
    private readonly SendGridClient _client;

    public SendGridMessageSender(IOptions<SendGridOptions> postmarkOptions)
    {
        _client = new(Guard.AgainstNull(Guard.AgainstNull(postmarkOptions).Value).ApiKey);
    }

    public string Channel => "email";
    public string Name => "sendgrid";

    public async Task SendAsync(Message message)
    {
        Guard.AgainstNull(message);

        var msg = new SendGridMessage
        {
            Subject = message.Subject
        };

        foreach (var recipient in message.Recipients)
        {
            switch (recipient.Type)
            {
                case RecipientType.To:
                {
                    msg.AddTo(recipient.Identifier);
                    break;
                }
                case RecipientType.Cc:
                {
                    msg.AddCc(recipient.Identifier);
                    break;
                }
                case RecipientType.Bcc:
                {
                    msg.AddBcc(recipient.Identifier);
                    break;
                }
            }
        }

        if (message.ContentType.Equals("text/html", StringComparison.InvariantCultureIgnoreCase))
        {
            msg.HtmlContent = message.Content;
        }
        else
        {
            msg.PlainTextContent = message.Content;
        }

        if (message.HasSender)
        {
            msg.From = new(message.Sender) ;
        }

        foreach (var attachment in message.GetAttachments())
        {
            msg.AddAttachment(new()
            {
                Content = Convert.ToBase64String(attachment.Content),
                Type = attachment.ContentType,
                Filename = attachment.Name,
                Disposition = "attachment"
            });
        }

        var response = await _client.SendEmailAsync(msg);

        if (!response.IsSuccessStatusCode)
        {
            throw new($"Failed to send email: {await response.Body.ReadAsStringAsync()}");
        }
    }
}