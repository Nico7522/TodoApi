using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Todo.Infrastructure.ChatHub.Models;

namespace Todo.Infrastructure.ChatHub;

public class Chat: Hub<IChat>
{
    public async Task JoinChatRoom(string teamId, string userId, List<UserStatus> userList)
    {   
        await Clients.Group(teamId).JoinChatRoom(userList); 
    }

    public async Task LeftChatRoom(string teamId, string userId, List<UserStatus> userList)
    {
        await Clients.Group(teamId).LeftChatRoom(userList);
    }

    public async Task SendMessage(string teamId, string message, string firstname, string lastname)
    {
        await Clients.Group(teamId).ReceiveMessage(message, firstname, lastname);
    }

}
