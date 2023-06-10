using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ToffApi.DtoModels;
using ToffApi.AuthenticationService;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using ToffApi.Models;

namespace ToffApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAccessTokenManager _accessTokenManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public AuthController(UserManager<User> userManager,
            RoleManager<ApplicationRole> roleManager,
            IAccessTokenManager accessTokenManager,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<User> signInManager,
            JwtSecurityTokenHandler tokenHandler)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._accessTokenManager = accessTokenManager;
            this._httpContextAccessor = httpContextAccessor;
            this._signInManager = signInManager;
            this._tokenHandler = tokenHandler;
        }

        [HttpPost("/auth/signup")]
        public async Task<IActionResult> CreateUser(UserDto user)
        {
            if (!ModelState.IsValid) return Ok();
            var appUser = new User
            {
                UserName = user.Name,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(appUser, user.Password);

            if (result.Succeeded)
            {
                const string message = "User Created Successfully";
                return Ok(message);
            }
            foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            return BadRequest(ModelState);
            
        }

        //[HttpPost("/auth/role")]
        //public async Task<IActionResult> CreateRole([Required] string name)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        IdentityResult result = await roleManager.CreateAsync(new ApplicationRole() { Name = name });
        //        if (result.Succeeded)
        //        {
        //            var Message = "Role Created Successfully";
        //            return Ok(Message);
        //        }
        //        else
        //        {
        //            foreach (IdentityError error in result.Errors)
        //                ModelState.AddModelError("", error.Description);
        //        }
        //    }
        //    return Ok();
        //}

        [HttpPost("/auth/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginInfo)
        {
            // example of how to retrieve user info from JWT
            if (_httpContextAccessor.HttpContext != null)
            {
                var tokenFromRequest = _httpContextAccessor.HttpContext.Request.Cookies["X-Access-Token"];
                if (!string.IsNullOrEmpty(tokenFromRequest))
                {
                    var userId = _tokenHandler.ReadJwtToken(tokenFromRequest).Claims.First(claim => claim.Type == "userId").Value;
                }
            }

            // END EXAMPLE

            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByEmailAsync(loginInfo.Email);
                if (appUser != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(appUser, loginInfo.Password, false, false);
                    if (result.Succeeded)
                    {
                        var token = _accessTokenManager.GenerateToken(appUser, new List<string>());
                        Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
                        return Ok(new { Result = result,
                                    username = appUser.UserName,
                                    email = appUser.Email,
                                    token = _accessTokenManager.GenerateToken(appUser, new List<string>())
                                });
                    }
                }
                ModelState.AddModelError(nameof(loginInfo.Email), "Login Failed: Invalid Email or Password");
            }
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }

        [Authorize]
        [HttpPut("/auth/logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }
    }
}
