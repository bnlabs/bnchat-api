using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace ToffApi.Controllers;

public abstract class Controller : ControllerBase
{
    protected Controller()
    {
        
    }
}