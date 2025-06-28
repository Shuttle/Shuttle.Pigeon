using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Postmark;

public class PostmarkBuilder
{
    private PostmarkOptions _postmarkOptions = new();

    public PostmarkBuilder(IServiceCollection services)
    {
        Services = Guard.AgainstNull(services);
    }

    public PostmarkOptions Options
    {
        get => _postmarkOptions;
        set => _postmarkOptions = Guard.AgainstNull(value);
    }

    public IServiceCollection Services { get; }
}