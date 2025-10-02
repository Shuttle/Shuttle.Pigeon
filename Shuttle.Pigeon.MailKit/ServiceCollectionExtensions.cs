using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.MailKit;

public static class ServiceCollectionExtensions
{
    public static PigeonBuilder TryAddMailKit(this PigeonBuilder pigeonBuilder, IConfiguration configuration)
    {
        var options = configuration.GetSection(MailKitOptions.SectionName).Get<MailKitOptions>();

        if (options != null)
        {
            pigeonBuilder.AddMailKit(builder => { builder.Options = options; });
        }

        return pigeonBuilder;
    }

    public static PigeonBuilder AddMailKit(this PigeonBuilder pigeonBuilder, Action<MailKitBuilder>? builder = null)
    {
        var mailKitBuilder = new MailKitBuilder(Guard.AgainstNull(pigeonBuilder).Services);

        builder?.Invoke(mailKitBuilder);

        pigeonBuilder.Services
            .AddSingleton<IMessageSender, MailKitMessageSender>()
            .AddOptions<MailKitOptions>().Configure(options =>
            {
                options.Host = mailKitBuilder.Options.Host;
                options.Port = mailKitBuilder.Options.Port;
                options.Username = mailKitBuilder.Options.Username;
                options.Password = mailKitBuilder.Options.Password;
                options.SenderAddress = mailKitBuilder.Options.SenderAddress;
                options.SenderDisplayName = mailKitBuilder.Options.SenderDisplayName;
            });

        return pigeonBuilder;
    }
}