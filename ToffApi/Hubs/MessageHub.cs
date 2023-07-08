using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using ToffApi.Command.CommandBuses;
using ToffApi.Command.CommandHandlers;
using ToffApi.Models;
using ToffApi.Services.DataAccess;
using ToffApi.Exceptions;
using ToffApi.Query.Queries;
using ToffApi.Query.QueryHandlers;
using ToffApi.Query.QueryResults;

namespace ToffApi.Hubs;

[Authorize]
public class MessageHub : ToffHub
{
    private readonly IMessageDataAccess _messageDataAccess;
    private readonly MessageCommandHandler _messageCommandHandler;
    private readonly MessageQueryHandler _messageQueryHandler;

    public MessageHub(
        JwtSecurityTokenHandler tokenHandler,
        IHttpContextAccessor httpContextAccessor,
        MessageCommandHandler messageCommandHandler,
        IMessageDataAccess messageDataAccess, 
        MessageQueryHandler messageQueryHandler) :
        base(tokenHandler, httpContextAccessor)
    {
        _messageDataAccess = messageDataAccess;
        _messageQueryHandler = messageQueryHandler;
        _messageCommandHandler = messageCommandHandler;
    }

    public async Task SendMessage(SendDmMessageCommand command)
    {
        var userId = ExtractUserId();
        
        var conversation = new GetConversationBetweenUsersQueryResult();
        var groupName = string.Empty;

        var getConversationQuery = new GetConversationBetweenUsersQuery(new Guid(userId), command.ReceiverId);
        // check if conversation already exist, if it does, then get conversation
        try
        {
            conversation = await _messageQueryHandler.HandleAsync(getConversationQuery);
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
            conversation = await _messageQueryHandler.HandleAsync(getConversationQuery);
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