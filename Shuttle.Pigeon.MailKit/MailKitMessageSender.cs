using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Shuttle.Contract;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Shuttle.Pigeon.MailKit;

public class MailKitMessageSender(IOptions<MailKitOptions> mailKitOptions) : IMessageSender
{
    private readonly MailKitOptions _mailKitOptions = Guard.AgainstNull(mailKitOptions).Value;

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
                    mimeMessage.To.Add(new MailboxAddress(recipient.DisplayName, recipient.Identifier));
                    break;
                }
                case RecipientType.Cc:
                {
                    mimeMessage.Cc.Add(new MailboxAddress(recipient.DisplayName, recipient.Identifier));
                    break;
                }
                case RecipientType.Bcc:
                {
                    mimeMessage.Bcc.Add(new MailboxAddress(recipient.DisplayName, recipient.Identifier));
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
            await bodyBuilder.Attachments.AddAsync(attachment.Name, new MemoryStream(attachment.Content), ContentType.Parse(attachment.ContentType), CancellationToken.None);
        }

        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        var dnsName = message.FindParameter("DnsName")?.GetValue<string>();

        if (!string.IsNullOrEmpty(dnsName))
        {
            client.ServerCertificateValidationCallback = (sender, certificate, _, errors) =>
            {
                if (errors == SslPolicyErrors.None)
                {
                    return true;
                }

                return certificate is X509Certificate2 cert &&
                       cert.GetNameInfo(X509NameType.DnsName, false).Equals(dnsName, StringComparison.OrdinalIgnoreCase);
            };
        }

        await client.ConnectAsync(message.FindParameter("Host")?.GetValue<string>() ?? _mailKitOptions.Host, message.FindParameter("Port")?.GetValue<int>() ?? _mailKitOptions.Port, SecureSocketOptions.Auto, cancellationToken); 
        await client.AuthenticateAsync(message.FindParameter("Username")?.GetValue<string>() ?? _mailKitOptions.Username, message.FindParameter("Password")?.GetValue<string>() ?? _mailKitOptions.Password, cancellationToken);
        await client.SendAsync(mimeMessage, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}