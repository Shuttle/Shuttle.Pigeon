using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.SendGrid;

public static class ServiceCollectionExtensions
{
    extension(PigeonBuilder pigeonBuilder)
    {
        public PigeonBuilder AddSendGrid(Action<SendGridBuilder>? builder = null)
        {
            var postmarkBuilder = new SendGridBuilder(Guard.AgainstNull(pigeonBuilder).Services);

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

        public PigeonBuilder TryAddSendGrid(IConfiguration configuration)
        {
            var options = configuration.GetSection(SendGridOptions.SectionName).Get<SendGridOptions>();

            if (options != null)
            {
                Guard.AgainstNull(pigeonBuilder).AddSendGrid(builder => { builder.Options = options; });
            }

            return pigeonBuilder;
        }
    }
}