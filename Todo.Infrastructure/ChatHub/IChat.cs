
namespace Todo.Infrastructure.ChatHub;

public interface IChat
{
    Task JoinChatRoom(string userId);
    Task ReceiveMessage(string message, string firstname, string lastname);
}
