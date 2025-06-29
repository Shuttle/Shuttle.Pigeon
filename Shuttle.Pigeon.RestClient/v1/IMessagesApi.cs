using Refit;
using Shuttle.Pigeon.Messages.v1;

namespace Shuttle.Pigeon.RestClient.v1;

public interface IMessagesApi
{
    [Post("/v1/messages/send")]
    Task SendAsync(SendMessage model);
    [Post("/v1/messages")]
    Task RegisterAsync(SendMessage model);
    [Post("/v1/message/{id}/attachment")]
    Task AddAttachmentAsync(Guid id, Attachment model);
    [Post("/v1/message/{id}/send")]
    Task SendAsync(Guid id);
}