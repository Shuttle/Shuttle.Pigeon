using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;

namespace Shuttle.Pigeon;

public class MessageService : IMessageService
{
    private readonly List<IMessageSender> _messageSenders;
    private readonly PigeonOptions _pigeonOptions;

    public MessageService(IOptions<PigeonOptions> pigeonOptions, IEnumerable<IMessageSender> messageSenders)
    {
        _pigeonOptions = Guard.AgainstNull(Guard.AgainstNull(pigeonOptions).Value);
        _messageSenders = Guard.AgainstNull(messageSenders).ToList();
    }

    public async Task SendAsync(Message message)
    {
        await GetMessageSender(Guard.AgainstEmpty(Guard.AgainstNull(message).Channel), message.MessageSenderName).SendAsync(message);
    }

    private IMessageSender GetMessageSender(string channel, string messageSenderName)
    {
        var name = string.IsNullOrWhiteSpace(messageSenderName)
            ? _pigeonOptions.ChannelDefaultMessageSenders.FirstOrDefault(item =>
                item.Channel.Equals(channel, StringComparison.InvariantCultureIgnoreCase))?.Name
            : messageSenderName;

        var messageSender = _messageSenders
            .FirstOrDefault(item =>
                item.Channel.Equals(channel, StringComparison.InvariantCultureIgnoreCase) &&
                (
                    string.IsNullOrWhiteSpace(name) ||
                    item.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)
                )
            );

        if (messageSender == null)
        {
            throw new ApplicationException(string.Format(Resources.MissingMessageSenderException, channel, string.IsNullOrWhiteSpace(messageSenderName) ? "(first)" : messageSenderName));
        }

        return messageSender;
    }
}