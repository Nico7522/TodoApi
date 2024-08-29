using Microsoft.AspNetCore.SignalR;

namespace Todo.Infrastructure.ChatHub;

public class Chat: Hub<IChat>
{

    public async Task JoinChatRoom(string teamId, string userId)
    {   
        await Clients.Group(teamId).JoinChatRoom(userId);
    }

    public async Task SendMessage(string teamId, string message, string firstname, string lastname)
    {
        await Clients.Group(teamId).ReceiveMessage(message, firstname, lastname);
    }

}
