using Microsoft.Extensions.DependencyInjection;
using Shuttle.Contract;

namespace Shuttle.Pigeon.Postmark;

public class PostmarkBuilder(IServiceCollection services)
{
    public PostmarkOptions Options
    {
        get;
        set => field = Guard.AgainstNull(value);
    } = new();

    public IServiceCollection Services { get; } = Guard.AgainstNull(services);
}