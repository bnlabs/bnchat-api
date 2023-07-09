using ToffApi.DtoModels;
using ToffApi.Models;
using ToffApi.Query.Queries;
using ToffApi.Query.QueryResults;
using ToffApi.Services.DataAccess;
using Exception = System.Exception;

namespace ToffApi.Query.QueryHandlers;

public class MessageQueryHandler : QueryHandler
{
    private readonly IMessageDataAccess _messageDataAccess;
    private readonly IUserDataAccess _userDataAccess;

    public MessageQueryHandler(IUserDataAccess userDataAccess, IMessageDataAccess messageDataAccess) : base()
    {
        _userDataAccess = userDataAccess;
        _messageDataAccess = messageDataAccess;
    }

    public async Task<GetConversationByIdQueryResult> HandleAsync(GetConversationByIdQuery query)
    {
        var resultList = await _messageDataAccess.GetConversationById(query.ConversationId);
        if (resultList.Count < 1) throw new Exception("No Conversation Found");
        var conversation = resultList[0];
        var getConversationQueryResult = new GetConversationByIdQueryResult()
        {
            ConversationId = conversation.ConversationId,
            MemberIds = conversation.MemberIds,
            Messages = conversation.Messages
        };
        return getConversationQueryResult;
    }

    public async Task<GetConversationsByUserIdQueryResult> HandleAsync(GetConversationsByUserIdQuery query)
    {
        var conversations = await _messageDataAccess.GetConversationByUserId(query.UserId);
        var conversationResultList = conversations.Select(c => new ConversationDto
        {
            ConversationId = c.ConversationId,
            MemberIds = c.MemberIds
        }).ToList();
        
        foreach (var c in conversationResultList)
        {
            var memberMap = c.MemberIds.ToDictionary(
                id => id,
                id => _userDataAccess.GetUserById(id)[0].UserName
            );
            c.MemberMap = memberMap;
            c.Messages = await _messageDataAccess.GetMessagesFromConversation(query.UserId, c.ConversationId);
            c.Messages = c.Messages.OrderByDescending(m => m.Timestamp).ToList();
        }
        
        var conversationsByUserIdResult = new GetConversationsByUserIdQueryResult()
        {
            ConversationList = conversationResultList
        };

        return conversationsByUserIdResult;
    }
    
    public async Task<GetConversationBetweenUsersQueryResult> HandleAsync(GetConversationBetweenUsersQuery query)
    {
        var conversation = await _messageDataAccess.GetConversationBetweenUsers(query.UserId1, query.UserId2);
        var conversationQueryResult = new GetConversationBetweenUsersQueryResult()
        {
            MemberIds = conversation.MemberIds,
            Messages = conversation.Messages,
            ConversationId = conversation.ConversationId
        };
        return conversationQueryResult;
    }
    
}