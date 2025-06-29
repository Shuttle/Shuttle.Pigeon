using Refit;
using Shuttle.Core.Contract;
using Shuttle.Pigeon.RestClient.v1;

namespace Shuttle.Pigeon.RestClient
{
    public class PigeonClient : IPigeonClient
    {
        public PigeonClient(HttpClient httpClient)
        {
            Guard.AgainstNull(httpClient);

            Messageses = RestService.For<IMessagesApi>(httpClient);
        }

        public IMessagesApi Messageses { get; }
    }
}