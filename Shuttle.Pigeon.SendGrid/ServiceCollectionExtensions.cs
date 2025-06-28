using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.SendGrid;

public static class ServiceCollectionExtensions
{
    public static PigeonBuilder TryAddSendGrid(this PigeonBuilder pigeonBuilder, IConfiguration configuration)
    {
        var options = configuration.GetSection(SendGridOptions.SectionName).Get<SendGridOptions>();

        if (options != null)
        {
            pigeonBuilder.AddSendGrid(builder => { builder.Options = options; });
        }

        return pigeonBuilder;
    }

    public static PigeonBuilder AddSendGrid(this PigeonBuilder pigeonBuilder, Action<SendGridBuilder>? builder = null)
    {
        if (pigeonBuilder == null)
        {
            throw new ArgumentNullException(nameof(pigeonBuilder));
        }

        var postmarkBuilder = new SendGridBuilder(pigeonBuilder.Services);

        builder?.Invoke(postmarkBuilder);

        pigeonBuilder.Services
            .AddSingleton<IValidateOptions<SendGridOptions>, SendGridOptionsValidator>()
            .AddSingleton<IMessageSender, SendGridMessageSender>()
            .AddOptions<SendGridOptions>().Configure(options =>
            {
                options.ApiKey = postmarkBuilder.Options.ApiKey;
            });

        return pigeonBuilder;
    }
}