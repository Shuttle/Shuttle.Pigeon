using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Smtp;

public class SmtpMessageSender : IMessageSender
{
    private readonly SmtpOptions _smtpOptions;

    public SmtpMessageSender(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = Guard.AgainstNull(Guard.AgainstNull(smtpOptions).Value);
    }

    public string Channel => "email";

    public async Task SendAsync(Message message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        using (var client = new SmtpClient(message.FindParameter("Host")?.GetValue<string>() ?? _smtpOptions.Host, message.FindParameter("Port")?.GetValue<int>() ?? _smtpOptions.Port))
        {
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(message.FindParameter("Username")?.GetValue<string>() ?? _smtpOptions.Username, message.FindParameter("Password")?.GetValue<string>() ?? _smtpOptions.Password);

            var mail = new MailMessage
            {
                Subject = message.Subject,
                Body = message.Content,
                IsBodyHtml = message.ContentType.Equals("text/html", StringComparison.InvariantCultureIgnoreCase)
            };

            if (message.HasSender)
            {
                mail.From = new(message.Sender, message.SenderDisplayName);
            }
            else
            {
                mail.From = new(_smtpOptions.SenderAddress, _smtpOptions.SenderDisplayName);
            }

            foreach (var recipient in message.Recipients)
            {
                switch (recipient.Type)
                {
                    case RecipientType.To:
                    {
                        mail.To.Add(recipient.Identifier);
                        break;
                    }
                    case RecipientType.Cc:
                    {
                        mail.CC.Add(recipient.Identifier);
                        break;
                    }
                    case RecipientType.Bcc:
                    {
                        mail.Bcc.Add(recipient.Identifier);
                        break;
                    }
                }
            }

            foreach (var attachment in message.GetAttachments())
            {
                using (var stream = new MemoryStream(attachment.Content))
                {
                    mail.Attachments.Add(new(stream, attachment.ContentType));
                }
            }

            client.Send(mail);
        }

        await Task.CompletedTask;
    }
}