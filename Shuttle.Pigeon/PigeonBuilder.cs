using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon;

public class PigeonBuilder
{
    private PigeonOptions _options = new();

    public IServiceCollection Services { get; }

    public PigeonOptions Options
    {
        get => _options;
        set => _options = Guard.AgainstNull(value);
    }

    public PigeonBuilder(IServiceCollection services)
    {
        Services = Guard.AgainstNull(services);
    }
}