namespace Shuttle.Pigeon;

public interface IMessageSender
{
    string Channel { get; }

    Task SendAsync(Message message);
}