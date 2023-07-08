using ToffApi.DtoModels;

namespace ToffApi.Query.QueryResults;

public class GetUsersByIdsQueryResult : QueryResult
{
    public List<UserDto> ListOfUsers { get; set; }
}