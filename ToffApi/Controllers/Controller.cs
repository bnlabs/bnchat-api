using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace ToffApi.Controllers;

public abstract class Controller : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    protected Controller(JwtSecurityTokenHandler tokenHandler, IHttpContextAccessor httpContextAccessor)
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