
namespace ToffApi.Query.Queries;

public class GetConversationBetweenUsersQuery : Query
{
    public GetConversationBetweenUsersQuery(IHttpContextAccessor callerContext, Guid userId1, Guid userId2) : base(callerContext)
    {

    }

    public Guid UserId1;
    public Guid UserId2;
    


}