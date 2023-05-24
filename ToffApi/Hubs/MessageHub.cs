using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using ToffApi.DtoModels;

namespace ToffApi.Hubs;

public class MessageHub : Hub
{
    public async Task AnnounceNewMessage(MessageDto msg)
    {
        var groupName = $"conversation-{msg.ConversationId}";
        
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("ReceiveMessage", msg);
        await Clients.All.SendAsync("messageReceive", msg);
    }

}