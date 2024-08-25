﻿using Microsoft.AspNetCore.SignalR;

namespace Todo.Infrastructure.ChatHub;

public class Chat: Hub<IChat>
{

    public async Task JoinChatRoom(string teamId, string message)
    {   
        await Clients.Group(teamId).JoinChatRoom(message);
    }

    public async Task SendMessage(string teamId, string message, string firstname, string lastname)
    {
        await Clients.Group(teamId).ReceiveMessage(message, firstname, lastname);
    }

}
