using ToffApi.DtoModels;

namespace ToffApi.Query.QueryResults;

public class GetConversationsByUserIdQueryResult : QueryResult
{
    public List<ConversationDto> ConversationList;
}