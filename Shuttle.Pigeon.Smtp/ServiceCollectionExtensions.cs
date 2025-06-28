using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Smtp;

public static class ServiceCollectionExtensions
{
    public static PigeonBuilder TryAddSmtp(this PigeonBuilder pigeonBuilder, IConfiguration configuration)
    {
        var options = configuration.GetSection(SmtpOptions.SectionName).Get<SmtpOptions>();

        if (options != null)
        {
            pigeonBuilder.AddSmtp(builder => { builder.Options = options; });
        }

        return pigeonBuilder;
    }

    public static PigeonBuilder AddSmtp(this PigeonBuilder pigeonBuilder, Action<SmtpBuilder>? builder = null)
    {
        var smtpBuilder = new SmtpBuilder(Guard.AgainstNull(pigeonBuilder).Services);

        builder?.Invoke(smtpBuilder);

        pigeonBuilder.Services
            .AddSingleton<IMessageSender, SmtpMessageSender>()
            .AddOptions<SmtpOptions>().Configure(options =>
            {
                options.Host = smtpBuilder.Options.Host;
                options.Port = smtpBuilder.Options.Port;
                options.Username = smtpBuilder.Options.Username;
                options.Password = smtpBuilder.Options.Password;
                options.SenderAddress = smtpBuilder.Options.SenderAddress;
                options.SenderDisplayName = smtpBuilder.Options.SenderDisplayName;
            });

        return pigeonBuilder;
    }
}