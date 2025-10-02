namespace Shuttle.Pigeon;

public interface IMessageSender
{
    string Channel { get; }
    string Name { get; }

    Task SendAsync(Message message, CancellationToken cancellationToken = default);
}