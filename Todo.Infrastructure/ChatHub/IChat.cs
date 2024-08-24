
namespace Todo.Infrastructure.ChatHub;

public interface IChat
{
    Task JoinChatRoom(string joinAlert);
    Task ReceiveMessage(string message, string firstname, string lastname);
}
