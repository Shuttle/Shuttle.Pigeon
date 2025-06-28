using Microsoft.Extensions.DependencyInjection;

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

        services.AddSingleton<IMessageService, MessageService>();

        return services;
    }
}