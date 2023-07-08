using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;

namespace ToffApi.Hubs;

public class ToffHub : Hub
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    
    public ToffHub(JwtSecurityTokenHandler tokenHandler, IHttpContextAccessor httpContextAccessor) : base()
    {
        _tokenHandler = tokenHandler;
        _httpContextAccessor = httpContextAccessor;
    }
    internal string ExtractUserId()
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        var tokenFromRequest = _httpContextAccessor?.HttpContext?.Request.Cookies["X-Access-Token"];
        
        if (string.IsNullOrEmpty(tokenFromRequest))
        {
            throw new UnauthorizedAccessException();
        }
        
        var userId = _tokenHandler.ReadJwtToken(tokenFromRequest).Claims.First(claim => claim.Type == "userId").Value;
        
        return userId;
    }

}