using ToffApi.DtoModels;

namespace ToffApi.Query.QueryResults;

public class SearchUserByUsernameQueryResult : QueryResult
{
    public List<UserDto> ListOfUsers { get; set; }
}