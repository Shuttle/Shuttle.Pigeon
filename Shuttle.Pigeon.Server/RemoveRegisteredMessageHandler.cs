using Microsoft.EntityFrameworkCore;
using Shuttle.Core.Contract;
using Shuttle.Esb;
using Shuttle.Extensions.EFCore;
using Shuttle.Pigeon.Data;
using Shuttle.Pigeon.Messages.v1;

namespace Shuttle.Pigeon.Server;

public class RemoveRegisteredMessageHandler : IMessageHandler<RemoveRegisteredMessage>
{
    private readonly IDbContextFactory<PigeonDbContext> _dbContextFactory;

    public RemoveRegisteredMessageHandler(IDbContextFactory<PigeonDbContext> dbContextFactory)
    {
        _dbContextFactory = Guard.AgainstNull(dbContextFactory);
    }

    public async Task ProcessMessageAsync(IHandlerContext<RemoveRegisteredMessage> context)
    {
        var message = Guard.AgainstNull(context).Message;

        using var scope = new DbContextScope();
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var model = dbContext.Messages.SingleOrDefault(item => item.Id == message.Id);

        if (model == null)
        {
            return;
        }

        dbContext.Messages.Remove(model);

        await dbContext.SaveChangesAsync();
    }
}