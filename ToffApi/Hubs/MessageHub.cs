using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using ToffApi.Command.CommandBuses;
using ToffApi.Command.CommandHandlers;
using ToffApi.Services.DataAccess;

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
        command.SenderId = new Guid(userId);

        var commandResult = await _messageCommandHandler.HandleAsync(command);
        var groupName = $"conversation-{commandResult.ConversationId}";

        await Clients.Group(groupName).SendAsync("ReceiveMessage", commandResult);
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