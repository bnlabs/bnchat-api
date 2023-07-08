using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using ToffApi.Command.CommandBuses;
using ToffApi.Command.CommandHandlers;
using ToffApi.Models;
using ToffApi.Services.DataAccess;
using ToffApi.Exceptions;

namespace ToffApi.Hubs;

[Authorize]
public class MessageHub : ToffHub
{
    private readonly IMessageDataAccess _messageDataAccess;
    private readonly MessageCommandHandler _messageCommandHandler;

    public MessageHub(
        JwtSecurityTokenHandler tokenHandler,
        IHttpContextAccessor httpContextAccessor,
        MessageCommandHandler messageCommandHandler,
        IMessageDataAccess messageDataAccess) :
        base(tokenHandler, httpContextAccessor)
    {
        _messageDataAccess = messageDataAccess;
        _messageCommandHandler = messageCommandHandler;
    }

    public async Task SendMessage(SendDmMessageCommand command)
    {
        var userId = ExtractUserId();
        
        var conversation = new Conversation();
        var groupName = string.Empty;
        
        // check if conversation already exist, if it does, then get conversation
        try
        {
            conversation = await _messageCommandHandler.GetConversationBetweenUsers(new Guid(userId), command.ReceiverId);
            groupName = $"conversation-{conversation.ConversationId}";
        
            var msg = new Message()
            {
                ConversationId = conversation.ConversationId,
                SenderId = command.SenderId,
                SenderName = command.SenderName,
                Content = command.Content
            };
        
            await _messageDataAccess.AddMessage(msg);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", msg);
        }
        catch (ConversationNotFoundException e)
        {
            var memberList = new List<Guid>
            {
                new Guid(userId),
                command.ReceiverId
            };

            var c = new Conversation(memberList);
            await _messageDataAccess.AddConversation(c);
            conversation = await _messageCommandHandler.GetConversationBetweenUsers(new Guid(userId), command.ReceiverId);
            groupName = $"conversation-{conversation.ConversationId}";
            
            var msg = new Message()
            {
                ConversationId = conversation.ConversationId,
                SenderId = command.SenderId,
                SenderName = command.SenderName,
                Content = command.Content
            };
            await _messageDataAccess.AddMessage(msg);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", msg);
        }
    }

    public async Task JoinGroup(Guid conversationId)
    {
        var userId = ExtractUserId();
        var conversations = await _messageDataAccess.GetConversationById(conversationId);
        var userIdIsInConversation = conversations[0].MemberIds.Exists(id => id == new Guid(userId));
        
        if (!userIdIsInConversation)
        {
            throw new UnauthorizedAccessException();
        }

        var groupName = $"conversation-{conversationId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("UserJoined", groupName);
    }

}