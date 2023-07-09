namespace ToffApi.Query.Queries;

public class GetConversationByIdQuery : Query
{
    public GetConversationByIdQuery(Guid conversationId) : base()
    {
        ConversationId = conversationId;
    }
    
    public Guid ConversationId;
    
}