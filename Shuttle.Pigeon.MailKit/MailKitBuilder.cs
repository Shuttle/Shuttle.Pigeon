using Microsoft.Extensions.DependencyInjection;
using Shuttle.Contract;

namespace Shuttle.Pigeon.MailKit;

public class MailKitBuilder(IServiceCollection services)
{
    public MailKitOptions Options
    {
        get;
        set => field = Guard.AgainstNull(value);
    } = new();

    public IServiceCollection Services { get; } = Guard.AgainstNull(services);
}