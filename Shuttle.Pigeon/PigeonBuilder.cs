using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon;

public class PigeonBuilder
{
    public IServiceCollection Services { get; }

    public PigeonBuilder(IServiceCollection services)
    {
        Services = Guard.AgainstNull(services);
    }
}