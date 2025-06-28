using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Data;

public class PigeonDataBuilder
{
    private PigeonDataOptions _pigeonDataOptions = new();

    public PigeonDataBuilder(IServiceCollection services)
    {
        Services = Guard.AgainstNull(services);
    }

    public PigeonDataOptions Options
    {
        get => _pigeonDataOptions;
        set => _pigeonDataOptions = Guard.AgainstNull(value);
    }

    public IServiceCollection Services { get; }
}