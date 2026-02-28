using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.RestClient;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPigeonClient(Action<PigeonClientBuilder>? builder = null)
        {
            var restClientBuilder = new PigeonClientBuilder(Guard.AgainstNull(services));

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
}