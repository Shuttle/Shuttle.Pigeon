using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.RestClient
{
    public class PigeonClientBuilder(IServiceCollection services)
    {
        public PigeonClientOptions Options
        {
            get;
            set => field = Guard.AgainstNull(value);
        } = new();

        public IServiceCollection Services { get; } = Guard.AgainstNull(services);
    }
}