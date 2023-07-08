namespace ToffApi.Query.Queries;

public class GetConversationsByUserIdQuery : Query
{
    public GetConversationsByUserIdQuery(Guid userId) : base()
    {
        UserId = userId;
    }

    public Guid UserId;

}