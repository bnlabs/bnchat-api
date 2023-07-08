using ToffApi.DtoModels;
using ToffApi.Exceptions;
using ToffApi.Query.Queries;
using ToffApi.Query.QueryResults;
using ToffApi.Services.DataAccess;

namespace ToffApi.Query.QueryHandlers;

public class UserQueryHandler : QueryHandler
{
    private readonly IUserDataAccess _userDataAccess;

    public UserQueryHandler(IUserDataAccess userDataAccess) : base()
    {
        _userDataAccess = userDataAccess;
    }

    public async Task<GetUserByIdQueryResult> HandleAsync(GetUserByIdQuery query)
    {
        var users = await _userDataAccess.GetUserByIdAsync(query.UserId);
        if (users.Count < 1)
        {
            throw new UserNotFoundException();
        }
        var resultUser = new UserDto()
        {
            Name = users[0].UserName,
            Id = users[0].Id,
            PictureUrl = users[0].PictureUrl
        };

        var queryResult = new GetUserByIdQueryResult() { User = resultUser };
        return queryResult;
    }

    public async Task<GetUsersByIdsQueryResult> HandleAsync(GetUsersByIdsQuery query)
    {
        var result = new List<UserDto>();
        foreach (var id in query.ListOfUserIds.Distinct())
        {
            var userList = await _userDataAccess.GetUserByIdAsync(id);
            var user = userList[0];
            var userDto = new UserDto()
            {
                Id = user.Id,
                Name = user.UserName,
                PictureUrl = user.PictureUrl
            };
            result.Add(userDto);
        }

        var queryResult = new GetUsersByIdsQueryResult() { ListOfUsers = result };
        return queryResult;
    }
    public async Task<SearchUserByUsernameQueryResult> HandleAsync(SearchUserByUsernameQuery query)
    {
        var queryResult = new SearchUserByUsernameQueryResult();
        var users = await _userDataAccess.SearchUser(query.searchQuery);
        var result = users.Select(user => new UserDto
        {
            Id = user.Id,
            Name = user.UserName,
            PictureUrl = user.PictureUrl
        }).ToList();

        queryResult.ListOfUsers = result;

        return queryResult;
    }
}