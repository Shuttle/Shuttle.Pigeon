using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Shuttle.Pigeon.Postmark;

namespace Shuttle.Pigeon.Tests;

[TestFixture]
public class PostmarkMessageSenderFixture
{
    [Test]
    [Category("integration")]
    public void Should_be_able_to_send_a_mail()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PostmarkMessageSenderFixture>()
            .Build();

        var options = configuration.GetSection(PostmarkOptions.SectionName).Get<PostmarkOptions>()!;
        var mailOptions = configuration.GetSection(MailOptions.SectionName).Get<MailOptions>()!;

        var sender = new PostmarkMessageSender(Options.Create(options));

        Assert.That(async () => await sender.SendAsync(
            new Message(Guid.NewGuid(), "email", "<h1>Hello</h1> <b>World</b>", "text/html")
                .AddRecipient(new(mailOptions.Recipient, RecipientType.To))
                .WithSender(mailOptions.Sender)
                .WithSubject($"Sent from {nameof(PostmarkMessageSenderFixture)}")), Throws.Nothing);
    }
}