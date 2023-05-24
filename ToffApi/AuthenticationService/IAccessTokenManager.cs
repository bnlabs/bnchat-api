using ToffApi.Models;

namespace ToffApi.AuthenticationService
{
    public interface IAccessTokenManager
    {
        string GenerateToken(User user, IList<string> roles);
        Task<bool> IsCurrentActiveToken();
        Task<bool> IsActiveAsync(string token);
    }
}
