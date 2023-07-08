using ToffApi.Models;

namespace ToffApi.Query.QueryResults;

public class GetConversationBetweenUsersQueryResult : QueryResult
{
    public Guid ConversationId;
    public List<Guid> MemberIds;
    public List<Message> Messages;
}