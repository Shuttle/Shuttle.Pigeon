using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon;

public class PigeonBuilder(IServiceCollection services)
{
    private PigeonOptions _options = new();

    public IServiceCollection Services { get; } = Guard.AgainstNull(services);

    public PigeonOptions Options
    {
        get => _options;
        set => _options = Guard.AgainstNull(value);
    }
}