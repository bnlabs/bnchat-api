using Toff.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ToffApi.DtoModels;
using ToffApi.AuthenticationService;
using Microsoft.AspNetCore.Authorization;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ToffApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IAccessTokenManager accessTokenManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly SignInManager<User> signInManager;

        public AuthController(UserManager<User> userManager,
            RoleManager<ApplicationRole> roleManager,
            IAccessTokenManager accessTokenManager,
            IHttpContextAccessor httpContextAccessor,
            SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.accessTokenManager = accessTokenManager;
            this.httpContextAccessor = httpContextAccessor;
            this.signInManager = signInManager;
        }

        [HttpPost("/auth/signup")]
        public async Task<IActionResult> CreateUser(UserDto user)
        {

            if (ModelState.IsValid)
            {
                var appUser = new User
                {
                    UserName = user.Name,
                    Email = user.Email
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    string message = "User Created Successfully";
                    return Ok(message);
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                    return BadRequest(ModelState);
                }
            }
            return Ok();
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
        public async Task<IActionResult> Login([Required][EmailAddress] string email, [Required] string password, string returnurl)
        {
            if (ModelState.IsValid)
            {
                User appUser = await userManager.FindByEmailAsync(email);
                if (appUser != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, password, false, false);
                    if (result.Succeeded)
                    {
                        var token = accessTokenManager.GenerateToken(appUser, new List<string>());
                        Response.Cookies.Append("X-Access-Token", token, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
                        return Ok(new { Result = result,
                                    username = appUser.UserName,
                                    email = appUser.Email,
                                    token = accessTokenManager.GenerateToken(appUser, new List<string>())
                                });
                    }
                }
                ModelState.AddModelError(nameof(email), "Login Failed: Invalid Email or Password");
            }
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }

        [Authorize]
        [HttpPut("/auth/logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return Ok();
        }
    }
}
