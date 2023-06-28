using ToffApi.Models;

namespace ToffApi.Services.DataAccess;

public interface IMessageDataAccess
{
    Task<List<Message>> GetMessagesFromConversation(Guid userId, Guid conversationId);
    Task AddMessage(Message msg);
    Task AddConversation(Conversation conversation);
    Task<List<Conversation>> GetConversationByUserId(Guid userId);
    Task<List<Conversation>> GetConversationById(Guid conversationId);
}