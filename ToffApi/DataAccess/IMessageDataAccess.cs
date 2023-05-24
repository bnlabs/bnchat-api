using ToffApi.Models;

namespace ToffApi.DataAccess;

public interface IMessageDataAccess
{
    Task<List<Message>> GetMessagesFromConversation(Guid userId, Guid conversationId);
    Task AddMessage(Message msg);
}