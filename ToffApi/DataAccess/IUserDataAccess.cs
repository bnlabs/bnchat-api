using ToffApi.Models;

namespace ToffApi.DataAccess;

public interface IUserDataAccess
{
    Task<List<User>> GetUserByIdAsync(Guid userId);
    List<User> GetUserById(Guid userId);
    Task<List<User>> SearchUser(string searchInput);
}