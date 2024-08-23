using Microsoft.AspNetCore.SignalR;

namespace Todo.Infrastructure.ChatHub;

public class Chat: Hub
{
    public async Task JoinChatRoom(string teamId, string firstname, string lastname)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, teamId);
        await Clients.Group(teamId).SendAsync("OnJoinChatRoom", $"{firstname} {lastname} has joined chat");
    }

    public async Task SendMessage(string teamId, string message, string firstname, string lastname)
    {
        await Clients.Group(teamId).SendAsync("onMessageReceived", message, firstname, lastname);
    }
}
