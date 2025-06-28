using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.Postmark;

public static class ServiceCollectionExtensions
{
    public static PigeonBuilder TryAddPostmark(this PigeonBuilder pigeonBuilder, IConfiguration configuration)
    {
        var options = configuration.GetSection(PostmarkOptions.SectionName).Get<PostmarkOptions>();

        if (options != null)
        {
            pigeonBuilder.AddPostmark(builder => { builder.Options = options; });
        }

        return pigeonBuilder;
    }

    public static PigeonBuilder AddPostmark(this PigeonBuilder pigeonBuilder, Action<PostmarkBuilder>? builder = null)
    {
        if (pigeonBuilder == null)
        {
            throw new ArgumentNullException(nameof(pigeonBuilder));
        }

        var postmarkBuilder = new PostmarkBuilder(pigeonBuilder.Services);

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