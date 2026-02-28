using Shuttle.Hopper;
using Shuttle.Pigeon.Messages.v1;
using Shuttle.Pigeon.SqlServer;

namespace Shuttle.Pigeon.Server;

public class RemoveRegisteredMessageHandler(PigeonDbContext dbContext) : IMessageHandler<RemoveRegisteredMessage>
{
    public async Task HandleAsync(RemoveRegisteredMessage message, CancellationToken cancellationToken = default)
    {
        var model = dbContext.Messages.SingleOrDefault(item => item.Id == message.Id);

        if (model == null)
        {
            return;
        }

        dbContext.Messages.Remove(model);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}