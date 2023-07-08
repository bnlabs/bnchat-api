namespace ToffApi.Query.Queries;

public class GetConversationByIdQuery : Query
{
    public GetConversationByIdQuery(IHttpContextAccessor callerContext, Guid conversationId) : base(callerContext)
    {
        ConversationId = conversationId;
    }
    
    public Guid ConversationId;
    
}