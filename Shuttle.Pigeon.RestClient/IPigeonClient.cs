using Shuttle.Pigeon.RestClient.v1;

namespace Shuttle.Pigeon.RestClient
{
    public interface IPigeonClient
    {
        IMessagesApi Messageses { get; }
    }
}