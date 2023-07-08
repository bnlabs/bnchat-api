using ToffApi.DtoModels;
using ToffApi.Models;
using ToffApi.Query.Queries;
using ToffApi.Query.QueryResults;
using ToffApi.Services.DataAccess;

namespace ToffApi.Command.CommandHandlers;

public class MessageCommandHandler : CommandHandler
{
    private readonly IMessageDataAccess _messageDataAccess;
    private readonly IUserDataAccess _userDataAccess;

    public MessageCommandHandler(IUserDataAccess userDataAccess, IMessageDataAccess messageDataAccess)
    {
        _userDataAccess = userDataAccess;
        _messageDataAccess = messageDataAccess;
    }


    public async Task<GetConversationByIdQueryResult> HandleAsync(GetConversationByIdQuery query)
    {
        var conversationList = await _messageDataAccess.GetConversationById(query.ConversationId);
        var conversation = conversationList.ToList()[0];
        var conversationQueryResult = new GetConversationByIdQueryResult()
        {
            ConversationId = conversation.ConversationId,
            MemberIds = conversation.MemberIds,
            Messages = conversation.Messages
        };

        return conversationQueryResult;
    }

    public async Task<GetConversationsByUserIdQueryResult> HandleAsync(GetConversationsByUserIdQuery query)
    {
        var conversationQueryResult = new GetConversationsByUserIdQueryResult();
        
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

        conversationQueryResult.ConversationList = conversationResultList;

        return conversationQueryResult;
    }
    public Task<Conversation> GetConversationBetweenUsers(Guid userId1, Guid userId2)
    {
        return _messageDataAccess.GetConversationBetweenUsers(userId1, userId2);
    }
    
    // TODO: test this
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