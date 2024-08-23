using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;

namespace Todo.Api.ChatHub
{
    public class Chat : Hub
    {
        public async Task JoinChatRoom(string teamId)
        {
            Console.WriteLine(teamId);
            await Clients.All.SendAsync("JoinChatRoom", $"Hello {teamId}");
            //await Groups.AddToGroupAsync(Context.ConnectionId, teamId.ToString());
            //await Clients.Group(teamId.ToString()).SendAsync("JoinChatRoom", "Joined chat room");
        }
    }
}


