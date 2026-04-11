namespace Shuttle.Pigeon.WebApi.Contracts.v1;

public class Attachment
{
    public string Name { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
}
