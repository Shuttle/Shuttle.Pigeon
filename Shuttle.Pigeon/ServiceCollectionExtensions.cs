using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPigeon(this IServiceCollection services, Action<PigeonBuilder>? builder = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var messageBuilder = new PigeonBuilder(services);

        builder?.Invoke(messageBuilder);

        services.TryAddSingleton<IValidateOptions<PigeonOptions>, PigeonOptionsValidator>();

        services.AddOptions<PigeonOptions>().Configure(options =>
        {
            options.ChannelDefaultMessageSenders = messageBuilder.Options.ChannelDefaultMessageSenders;
        });

        services.AddSingleton<IMessageService, MessageService>();

        return services;
    }
}