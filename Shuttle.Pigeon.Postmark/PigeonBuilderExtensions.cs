using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Postmark;

public static class PigeonBuilderExtensions
{
    extension(PigeonBuilder pigeonBuilder)
    {
        public PigeonBuilder TryAddPostmark(IConfiguration configuration)
        {
            var options = configuration.GetSection(PostmarkOptions.SectionName).Get<PostmarkOptions>();

            if (options != null)
            {
                Guard.AgainstNull(pigeonBuilder).AddPostmark(builder => { builder.Options = options; });
            }

            return pigeonBuilder;
        }

        public PigeonBuilder AddPostmark(Action<PostmarkBuilder>? builder = null)
        {
            var postmarkBuilder = new PostmarkBuilder(Guard.AgainstNull(pigeonBuilder).Services);

            builder?.Invoke(postmarkBuilder);

            pigeonBuilder.Services
                .AddSingleton<IValidateOptions<PostmarkOptions>, PostmarkOptionsValidator>()
                .AddSingleton<IMessageSender, PostmarkMessageSender>()
                .AddOptions<PostmarkOptions>().Configure(options =>
                {
                    options.ServerToken = postmarkBuilder.Options.ServerToken;
                });

            return pigeonBuilder;
        }
    }
}