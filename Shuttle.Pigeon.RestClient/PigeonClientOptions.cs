namespace Shuttle.Pigeon.RestClient;

public class PigeonClientOptions
{
    public const string SectionName = "Shuttle:Pigeon:Client";

    public Uri? BaseAddress { get; set; }
    public Func<HttpRequestMessage, IServiceProvider, Task>? ConfigureHttpRequestAsync { get; set; }
}