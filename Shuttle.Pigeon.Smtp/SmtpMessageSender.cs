using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Smtp;

public class SmtpMessageSender(IOptions<SmtpOptions> smtpOptions) : IMessageSender
{
    private readonly SmtpOptions _smtpOptions = Guard.AgainstNull(Guard.AgainstNull(smtpOptions).Value);

    public string Channel => "email";
    public string Name => "smtp";

    public async Task SendAsync(Message message, CancellationToken cancellationToken = default)
    {
        Guard.AgainstNull(message);

        using (var client = new SmtpClient(message.FindParameter("Host")?.GetValue<string>() ?? _smtpOptions.Host, message.FindParameter("Port")?.GetValue<int>() ?? _smtpOptions.Port))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(message.FindParameter("Username")?.GetValue<string>() ?? _smtpOptions.Username, message.FindParameter("Password")?.GetValue<string>() ?? _smtpOptions.Password);

            var msg = new MailMessage
            {
                Subject = message.Subject,
                Body = message.Content,
                IsBodyHtml = message.ContentType.Equals("text/html", StringComparison.InvariantCultureIgnoreCase)
            };

            if (message.HasSender)
            {
                msg.From = new(message.Sender, message.SenderDisplayName);
            }
            else
            {
                msg.From = new(_smtpOptions.SenderAddress, _smtpOptions.SenderDisplayName);
            }

            foreach (var recipient in message.Recipients)
            {
                switch (recipient.Type)
                {
                    case RecipientType.To:
                    {
                        msg.To.Add(recipient.Identifier);
                        break;
                    }
                    case RecipientType.Cc:
                    {
                        msg.CC.Add(recipient.Identifier);
                        break;
                    }
                    case RecipientType.Bcc:
                    {
                        msg.Bcc.Add(recipient.Identifier);
                        break;
                    }
                }
            }

            foreach (var attachment in message.GetAttachments())
            {
                using (var stream = new MemoryStream(attachment.Content))
                {
                    msg.Attachments.Add(new(stream, attachment.ContentType));
                }
            }

            client.Send(msg);
        }

        await Task.CompletedTask;
    }
}