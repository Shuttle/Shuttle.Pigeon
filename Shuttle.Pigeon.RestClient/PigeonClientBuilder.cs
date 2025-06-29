using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.RestClient
{
    public class PigeonClientBuilder
    {
        public PigeonClientOptions Options
        {
            get => _pigeonClientOptions;
            set => _pigeonClientOptions = Guard.AgainstNull(value);
        }

        private PigeonClientOptions _pigeonClientOptions = new();

        public PigeonClientBuilder(IServiceCollection services)
        {
            Services = Guard.AgainstNull(services);
        }

        public IServiceCollection Services { get; }
    }
}