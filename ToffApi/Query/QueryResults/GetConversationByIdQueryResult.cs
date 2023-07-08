using ToffApi.Models;

namespace ToffApi.Query.QueryResults;

public class GetConversationByIdQueryResult : QueryResult
{
    public Guid ConversationId{ get; set; }
    public List<Guid> MemberIds { get; set; }
    public List<Message> Messages { get; set; }
    
    
}