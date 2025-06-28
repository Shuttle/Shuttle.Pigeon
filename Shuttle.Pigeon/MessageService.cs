namespace Shuttle.Pigeon;

public class MessageService : IMessageService
{
    private readonly Dictionary<string, IMessageSender> _messageSenders;

    public MessageService(IEnumerable<IMessageSender> messageSenders)
    {
        (messageSenders ?? throw new ArgumentNullException(nameof(messageSenders)))
            .GroupBy(item => item.Channel.ToUpperInvariant())
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList()
            .ForEach(channel => throw new ArgumentException(string.Format(Resources.DuplicateChannelException, channel), nameof(messageSenders)));

        _messageSenders = messageSenders.ToDictionary(item => item.Channel.ToUpperInvariant());
    }

    public async Task SendAsync(Message message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        await GetMessageSender(message.Channel).SendAsync(message);
    }

    private IMessageSender GetMessageSender(string channel)
    {
        if (!_messageSenders.TryGetValue(channel.ToUpperInvariant(), out var sender))
        {
            throw new ArgumentException(string.Format(Resources.MissingChannelException, channel), nameof(channel));
        }

        return sender;
    }
}