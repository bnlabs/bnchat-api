using ToffApi.DtoModels;

namespace ToffApi.Query.QueryResults;

public class GetUserByIdQueryResult : QueryResult
{
    public UserDto User { get; set; }
}