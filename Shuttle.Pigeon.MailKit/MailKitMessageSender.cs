using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.MailKit;

public class MailKitMessageSender(IOptions<MailKitOptions> mailKitOptions) : IMessageSender
{
    private readonly MailKitOptions _mailKitOptions = Guard.AgainstNull(Guard.AgainstNull(mailKitOptions).Value);

    public string Channel => "email";
    public string Name => "mailKit";

    public async Task SendAsync(Message message, CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(message);

        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(message.HasSender ? new(message.SenderDisplayName, message.Sender) : new MailboxAddress(_mailKitOptions.SenderDisplayName, _mailKitOptions.SenderAddress));

        foreach (var recipient in message.Recipients)
        {
            switch (recipient.Type)
            {
                case RecipientType.To:
                {
                    mimeMessage.To.Add(new MailboxAddress(recipient.HasDisplayName ? recipient.DisplayName : recipient.Identifier, recipient.Identifier));
                    break;
                }
                case RecipientType.Cc:
                {
                    mimeMessage.Cc.Add(new MailboxAddress(recipient.HasDisplayName ? recipient.DisplayName : recipient.Identifier, recipient.Identifier));
                    break;
                }
                case RecipientType.Bcc:
                {
                    mimeMessage.Bcc.Add(new MailboxAddress(recipient.HasDisplayName ? recipient.DisplayName : recipient.Identifier, recipient.Identifier));
                    break;
                }
            }
        }
        mimeMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder();

        if (message.ContentType.Equals("text/html", StringComparison.InvariantCultureIgnoreCase))
        {
            bodyBuilder.HtmlBody = message.Content;
        }
        else
        {
            bodyBuilder.TextBody = message.Content;
        }

        foreach (var attachment in message.GetAttachments())
        {
            using var stream = new MemoryStream(attachment.Content);
            await bodyBuilder.Attachments.AddAsync(attachment.Name, stream, ContentType.Parse(attachment.ContentType), CancellationToken.None);
        }

        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(message.FindParameter("Host")?.GetValue<string>() ?? _mailKitOptions.Host, message.FindParameter("Port")?.GetValue<int>() ?? _mailKitOptions.Port, SecureSocketOptions.StartTls, cancellationToken); 
        await client.AuthenticateAsync(message.FindParameter("Username")?.GetValue<string>() ?? _mailKitOptions.Username, message.FindParameter("Password")?.GetValue<string>() ?? _mailKitOptions.Password, cancellationToken);
        await client.SendAsync(mimeMessage, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}