using System.Collections.Concurrent;
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
    private static ConcurrentDictionary<string, string> _connectedUsers = new ConcurrentDictionary<string, string>();
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

    public override Task OnConnectedAsync()
    {
        // Retrieve user information from the context
        var userId = ExtractUserId();
        var connectionId = Context.ConnectionId;

        // Add the user and connection ID to the dictionary
        _connectedUsers.TryAdd(userId, connectionId);
        
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception exception)
    {
        // Retrieve user information from the context
        var userId = ExtractUserId();

        // Remove the user from the dictionary
        _connectedUsers.TryRemove(userId, out _);

        return base.OnDisconnectedAsync(exception);
    }
    public async Task SendMessage(SendDmMessageCommand command)
    {
        var userId = ExtractUserId();
        command.SenderId = new Guid(userId);

        var commandResult = await _messageCommandHandler.HandleAsync(command);
        
        var groupName = $"conversation-{commandResult.ConversationId}";
        _connectedUsers.TryGetValue(commandResult.SenderId.ToString(), out var connectionId2);
        await Groups.AddToGroupAsync(connectionId2, groupName);

        if (commandResult.NewConversation && _connectedUsers.TryGetValue(commandResult.ReceiverId.ToString(), out var connectionId))
        {
            await Groups.AddToGroupAsync(connectionId, groupName);
        }

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