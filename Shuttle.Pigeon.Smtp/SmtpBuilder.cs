using Microsoft.Extensions.DependencyInjection;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.Smtp;

public class SmtpBuilder(IServiceCollection services)
{
    private SmtpOptions _smtpOptions = new();

    public SmtpOptions Options
    {
        get => _smtpOptions;
        set => _smtpOptions = Guard.AgainstNull(value);
    }

    public IServiceCollection Services { get; } = Guard.AgainstNull(services);
}