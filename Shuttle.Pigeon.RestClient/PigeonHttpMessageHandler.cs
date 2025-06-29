using System.Reflection;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon.RestClient;

public class PigeonHttpMessageHandler : DelegatingHandler
{
    private readonly PigeonClientOptions _pigeonClientOptions;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _userAgent;

    public PigeonHttpMessageHandler(IOptions<PigeonClientOptions> pigeonClientOptions, IServiceProvider serviceProvider)
    {
        _pigeonClientOptions = Guard.AgainstNull(Guard.AgainstNull(pigeonClientOptions).Value);
        _serviceProvider = Guard.AgainstNull(serviceProvider);

        var version = Assembly.GetExecutingAssembly().GetName().Version;

        _userAgent = $"Shuttle.Pigeon{(version != null ? $"/{version.Major}.{version.Minor}.{version.Build}" : string.Empty)}";
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Guard.AgainstNull(request);

        request.Headers.Add("User-Agent", _userAgent);

        await (_pigeonClientOptions.ConfigureHttpRequestAsync?.Invoke(request, _serviceProvider) ?? Task.CompletedTask);

        return await base.SendAsync(request, cancellationToken);
    }
}