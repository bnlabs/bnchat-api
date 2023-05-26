using ToffApi.Models;

namespace ToffApi.DataAccess;

public interface IUserDataAccess
{
    Task<List<User>> GetUserById(Guid userId);
}