
using Todo.Infrastructure.ChatHub.Models;

namespace Todo.Infrastructure.ChatHub;

public interface IChat
{
    Task JoinChatRoom(List<UserStatus> userList);

    Task LeftChatRoom(List<UserStatus> userList);
    Task ReceiveMessage(string message, string firstname, string lastname);
}
