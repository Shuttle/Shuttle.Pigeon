using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Shuttle.Pigeon.RestClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPigeonClient(this IServiceCollection services, Action<PigeonClientBuilder>? builder = null)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        var restClientBuilder = new PigeonClientBuilder(services);

        builder?.Invoke(restClientBuilder);

        services.AddOptions<PigeonClientOptions>().Configure(options => { options.BaseAddress = restClientBuilder.Options.BaseAddress; });

        services.TryAddSingleton<IPigeonClient, PigeonClient>();

        services.AddHttpClient<IPigeonClient, PigeonClient>("PigeonClient", (serviceProvider, client) =>
            {
                client.BaseAddress = serviceProvider.GetRequiredService<IOptions<PigeonClientOptions>>().Value.BaseAddress;
            })
            .AddHttpMessageHandler<PigeonHttpMessageHandler>();

        return services;
    }
}