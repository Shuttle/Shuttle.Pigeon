namespace Shuttle.Pigeon;

public interface IMessageService
{
    Task SendAsync(Message message);
}