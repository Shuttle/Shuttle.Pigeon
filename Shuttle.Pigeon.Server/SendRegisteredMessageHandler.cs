using Microsoft.EntityFrameworkCore;
using Shuttle.Core.Contract;
using Shuttle.Hopper;
using Shuttle.Pigeon.Messages.v1;
using Shuttle.Pigeon.SqlServer;

namespace Shuttle.Pigeon.Server;

public class SendRegisteredMessageHandler(PigeonDbContext dbContext, IMessageService messageService, IBus bus)
    : IMessageHandler<SendRegisteredMessage>
{
    private readonly PigeonDbContext _dbContext = Guard.AgainstNull(dbContext);
    private readonly IMessageService _messageService = Guard.AgainstNull(messageService);
    private readonly IBus _bus = Guard.AgainstNull(bus);

    public async Task HandleAsync(SendRegisteredMessage message, CancellationToken cancellationToken = default)
    {
        var model = _dbContext.Messages
            .Include(item => item.Recipients)
            .Include(item => item.Attachments)
            .SingleOrDefault(item => item.Id == message.Id);

        if (model == null)
        {
            return;
        }

        var pigeonMessage = new Message(message.Id, model.Channel, model.Content, model.ContentType)
            .AddRecipients(model.Recipients.Select(item => new Message.Recipient(item.Identifier, (RecipientType)item.Type)))
            .WithSubject(model.Subject)
            .WithSender(model.Sender);

        foreach (var attachment in model.Attachments)
        {
            pigeonMessage.AddAttachment(new(attachment.Name, attachment.ContentType, attachment.Content));
        }

        await _messageService.SendAsync(pigeonMessage);

        model.DateSent = DateTime.Now.ToUniversalTime();

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _bus.SendAsync(new RemoveRegisteredMessage { Id = message.Id }, builder => builder.ToSelf(), cancellationToken);
    }
}