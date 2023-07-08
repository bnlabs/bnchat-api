
namespace ToffApi.Query.Queries;

public class GetConversationBetweenUsersQuery : Query
{
    public GetConversationBetweenUsersQuery(Guid userId1, Guid userId2) : base()
    {
        UserId1 = userId1;
        UserId2 = userId2;
    }

    public Guid UserId1 { get; set; }
    public Guid UserId2 { get; set; }
    


}