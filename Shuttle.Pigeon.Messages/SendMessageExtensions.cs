using Shuttle.Core.Contract;
using Shuttle.Pigeon.Messages.v1;

namespace Shuttle.Pigeon.Messages;

public static class SendMessageExtensions
{
    public static SendMessage AddRecipient(this SendMessage message, string identifier, int type)
    {
        Guard.AgainstNull(message);

        message.Recipients.Add(new()
        {
            Identifier = Guard.AgainstEmpty(identifier),
            Type = type
        });

        return message;
    }
}