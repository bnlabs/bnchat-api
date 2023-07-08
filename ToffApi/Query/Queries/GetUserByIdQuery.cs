namespace ToffApi.Query.Queries;

public class GetUserByIdQuery : Query
{
    public Guid UserId { get; set; }
}