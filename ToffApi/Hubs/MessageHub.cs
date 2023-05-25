using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using ToffApi.DataAccess;
using ToffApi.DtoModels;
using ToffApi.Models;

namespace ToffApi.Hubs;

public class MessageHub : Hub
{
    private readonly IMessageDataAccess _messageDataAccess;
    // private readonly IMapper _mapper;

    public MessageHub(IMessageDataAccess messageDataAccess)
    {
        _messageDataAccess = messageDataAccess;
        // _mapper = mapper;
    }

    public async Task SendMessage(MessageDto msg)
    {
        var groupName = $"conversation-{msg.ConversationId}";
        // var mappedMessage = _mapper.Map<Message>(msg);
        
        // await _messageDataAccess.AddMessage(mappedMessage);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("ReceiveMessage", msg);
        // await Clients.All.SendAsync("messageReceive", msg);
    }

    public Task AddToGroup(Guid conversationId)
    {
        var groupName = $"conversation-{conversationId}";
        return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

}