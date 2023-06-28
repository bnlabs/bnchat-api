using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using ToffApi.Models;

namespace ToffApi.Services.AuthenticationService
{
    public class AccessTokenManager : IAccessTokenManager
    {
        private readonly IDistributedCache _accessTokensCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        public AccessTokenManager(IConfiguration config, IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _accessTokensCache = distributedCache;
            this._httpContextAccessor = httpContextAccessor;
        }
        public string GenerateToken(User user, IList<string> roles)
        {
            var claims = new List<Claim> {
            new Claim("userId", user.Id.ToString())
        };

            var key = Encoding.ASCII.GetBytes(_config["JWT:key"]);

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var accessToken = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(accessToken);
        }

        public async Task<bool> IsActiveAsync(string token)
        {
            return await _accessTokensCache.GetStringAsync(GetKey(token)) == null;
        }

        public async Task<bool> IsCurrentActiveToken() => await IsActiveAsync(GetCurrentAsync());

        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty ? string.Empty : authorizationHeader.Single().Split(" ")[authorizationHeader.Single().Split(" ").Length - 1];
        }

        private static string GetKey(string token) => $"tokens:{token}:deactivated";
    }
}
