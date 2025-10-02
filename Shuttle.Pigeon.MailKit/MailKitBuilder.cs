using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.MailKit;

public class MailKitBuilder(IServiceCollection services)
{
    private MailKitOptions _mailKitOptions = new();

    public MailKitOptions Options
    {
        get => _mailKitOptions;
        set => _mailKitOptions = Guard.AgainstNull(value);
    }

    public IServiceCollection Services { get; } = Guard.AgainstNull(services);
}