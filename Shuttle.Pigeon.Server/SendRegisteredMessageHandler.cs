using Microsoft.EntityFrameworkCore;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Extensions.EFCore;
using Shuttle.Pigeon.Data;
using Shuttle.Pigeon.Messages.v1;

namespace Shuttle.Pigeon.Server;

public class SendRegisteredMessageHandler : IMessageHandler<SendRegisteredMessage>
{
    private readonly IDbContextFactory<PigeonDbContext> _dbContextFactory;
    private readonly IMessageService _messageService;

    public SendRegisteredMessageHandler(IDbContextFactory<PigeonDbContext> dbContextFactory, IMessageService messageService)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
    }

    public async Task ProcessMessageAsync(IHandlerContext<SendRegisteredMessage> context)
    {
        var message = Guard.AgainstNull(context).Message;

        using var scope = new DbContextScope();
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var model = dbContext.Messages
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

        await dbContext.SaveChangesAsync();

        await context.SendAsync(new RemoveRegisteredMessage { Id = message.Id }, builder => builder.Local());
    }
}