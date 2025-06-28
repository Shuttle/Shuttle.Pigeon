using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Pigeon.Messages.v1;

namespace Shuttle.Pigeon.Server;

public class SendMessageHandler : IMessageHandler<SendMessage>
{
    private readonly IMessageService _messageService;

    public SendMessageHandler(IMessageService messageService)
    {
        _messageService = Guard.AgainstNull(messageService);
    }

    public async Task ProcessMessageAsync(IHandlerContext<SendMessage> context)
    {
        var message = Guard.AgainstNull(context).Message;

        var pigeonMessage = new Message(message.Id, message.Channel, message.Content, message.ContentType)
            .AddRecipients(message.Recipients.Select(item => new Message.Recipient(item.Identifier, (RecipientType)item.Type)))
            .WithSubject(message.Subject);

        if (!string.IsNullOrWhiteSpace(message.Sender))
        {
            pigeonMessage.WithSender(message.Sender);
        }

        await _messageService.SendAsync(pigeonMessage);
    }
}