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
    private readonly IMapper _mapper;
    private readonly IUserDataAccess _userDataAccess;

    public MessageHub(IMessageDataAccess messageDataAccess, IMapper mapper, IUserDataAccess userDataAccess)
    {
        _messageDataAccess = messageDataAccess;
        _mapper = mapper;
        _userDataAccess = userDataAccess;
    }

    public async Task SendMessage(MessageDto msg)
    {
        var groupName = $"conversation-{msg.ConversationId}";
        var mappedMessage = _mapper.Map<Message>(msg);

        await _messageDataAccess.AddMessage(mappedMessage);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("ReceiveMessage", new { message = mappedMessage, id = Context.ConnectionId });
        // await Clients.All.SendAsync("messageReceive", msg);
    }

    public async Task JoinGroup(Guid conversationId)
    {
        var groupName = $"conversation-{conversationId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("UserJoined", groupName);
    }

}