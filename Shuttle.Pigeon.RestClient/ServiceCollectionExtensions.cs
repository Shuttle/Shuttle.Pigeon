using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shuttle.Contract;

namespace Shuttle.Pigeon.RestClient;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPigeonClient(Action<PigeonClientOptions> configureOptions)
        {
            services.AddOptions<PigeonClientOptions>().Configure(configureOptions);

            services.TryAddSingleton<IPigeonClient, PigeonClient>();

            services.AddHttpClient<IPigeonClient, PigeonClient>("PigeonClient", (serviceProvider, client) =>
                {
                    client.BaseAddress = serviceProvider.GetRequiredService<IOptions<PigeonClientOptions>>().Value.BaseAddress;
                })
                .AddHttpMessageHandler<PigeonHttpMessageHandler>();

            return services;
        }
    }
}