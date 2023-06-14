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
        private readonly IAccessTokenManager _accessTokenManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(UserManager<User> userManager,
            IAccessTokenManager accessTokenManager,
            SignInManager<User> signInManager)
        {
            this._userManager = userManager;
            this._accessTokenManager = accessTokenManager;
            this._signInManager = signInManager;
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

        [HttpPost("/auth/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginInfo)
        {
            if (ModelState.IsValid)
            {
                var appUser = await _userManager.FindByEmailAsync(loginInfo.Email);
                if (appUser != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(appUser, loginInfo.Password, false, false);
                    if (result.Succeeded)
                    {
                        var token = _accessTokenManager.GenerateToken(appUser, new List<string>());
                        Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.None, Secure = true });
                        return Ok(new { Result = result,
                                    id = appUser.Id,
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
