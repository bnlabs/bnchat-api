using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ToffApi.DataAccess;
using ToffApi.DtoModels;
using ToffApi.Models;

namespace ToffApi.Hubs;

[Authorize]
public class MessageHub : Hub
{
    private readonly IMessageDataAccess _messageDataAccess;
    private readonly IMapper _mapper;
    private readonly IUserDataAccess _userDataAccess;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    public MessageHub(IMessageDataAccess messageDataAccess,
        IMapper mapper,
        IUserDataAccess userDataAccess, 
        IHttpContextAccessor httpContextAccessor,
        JwtSecurityTokenHandler tokenHandler)
    {
        _messageDataAccess = messageDataAccess;
        _mapper = mapper;
        _userDataAccess = userDataAccess;
        _httpContextAccessor = httpContextAccessor;
        _tokenHandler = tokenHandler;
    }

    public async Task SendMessage(MessageDto msg)
    {
        // Retrieve JWT from HTTP context to get userId
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        var tokenFromRequest = _httpContextAccessor.HttpContext.Request.Cookies["X-Access-Token"];
        if (string.IsNullOrEmpty(tokenFromRequest))
        {
            throw new UnauthorizedAccessException();
        }
        
        var userId = _tokenHandler.ReadJwtToken(tokenFromRequest).Claims.First(claim => claim.Type == "userId").Value;
        var conversations = await _messageDataAccess.GetConversationById(msg.ConversationId);
        var userIdIsInConversation = conversations[0].MemberIds.Exists(id => id == new Guid(userId));
        if (!userIdIsInConversation)
        {
            throw new UnauthorizedAccessException();
        }

        var groupName = $"conversation-{msg.ConversationId}";
        var mappedMessage = _mapper.Map<Message>(msg);

        await _messageDataAccess.AddMessage(mappedMessage);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("ReceiveMessage", mappedMessage);
    }

    public async Task JoinGroup(Guid conversationId)
    {
        // Retrieve JWT from HTTP context to get userId
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        var tokenFromRequest = _httpContextAccessor?.HttpContext?.Request.Cookies["X-Access-Token"];
        if (string.IsNullOrEmpty(tokenFromRequest))
        {
            throw new UnauthorizedAccessException();
        }
        
        var userId = _tokenHandler.ReadJwtToken(tokenFromRequest).Claims.First(claim => claim.Type == "userId").Value;
        var conversations = await _messageDataAccess.GetConversationById(conversationId);
        var userIdIsInConversation = conversations[0].MemberIds.Exists(id => id == new Guid(userId));
        
        if (!userIdIsInConversation)
        {
            throw new UnauthorizedAccessException();
        }
        //
        var groupName = $"conversation-{conversationId}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("UserJoined", groupName);
    }

}