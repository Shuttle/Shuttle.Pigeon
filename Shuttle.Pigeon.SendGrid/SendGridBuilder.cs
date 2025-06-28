using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.SendGrid;

public class SendGridBuilder
{
    private SendGridOptions _postmarkOptions = new();

    public SendGridBuilder(IServiceCollection services)
    {
        Services = Guard.AgainstNull(services);
    }

    public SendGridOptions Options
    {
        get => _postmarkOptions;
        set => _postmarkOptions = Guard.AgainstNull(value);
    }

    public IServiceCollection Services { get; }
}