using Microsoft.Extensions.DependencyInjection;
using Shuttle.Contract;

namespace Shuttle.Pigeon;

public class PigeonBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = Guard.AgainstNull(services);

    public PigeonOptions Options
    {
        get;
        set => field = Guard.AgainstNull(value);
    } = new();
}