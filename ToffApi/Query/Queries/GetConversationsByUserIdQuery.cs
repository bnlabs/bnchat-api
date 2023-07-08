namespace ToffApi.Query.Queries;

public class GetConversationsByUserIdQuery : Query
{
    public GetConversationsByUserIdQuery(IHttpContextAccessor callerContext, Guid userId) : base(callerContext)
    {
        UserId = userId;
    }

    public Guid UserId;

}