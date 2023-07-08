namespace ToffApi.Query.Queries;

public class GetUsersByIdsQuery : Query
{
    public List<Guid> ListOfUserIds { get; set; }
}