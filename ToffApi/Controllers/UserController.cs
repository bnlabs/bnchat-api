using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToffApi.DtoModels;
using ToffApi.Services.CloudFlareR2Service;
using ToffApi.Services.DataAccess;

namespace ToffApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserDataAccess _userDataAccess;
        private readonly IR2Service _r2Service;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserDataAccess userDataAccess, IR2Service r2Service, JwtSecurityTokenHandler tokenHandler, IHttpContextAccessor httpContextAccessor)
        {
            _userDataAccess = userDataAccess;
            _r2Service = r2Service;
            _tokenHandler = tokenHandler;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("getUserById")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var users = await _userDataAccess.GetUserByIdAsync(userId);
            var resultUser = new UserDto()
            {
                Name = users[0].UserName,
                Id = users[0].Id,
                PictureUrl = users[0].PictureUrl
            };
            return Ok(resultUser);
        }

        [HttpPost("getUsers")]
        public async Task<IActionResult> GetUsersById([FromBody]List<Guid> listOfUserId)
        {
            var result = new List<UserDto>();
            foreach (var id in listOfUserId.Distinct())
            {
                var userList = await _userDataAccess.GetUserByIdAsync(id);
                var user = userList[0];
                var userDto = new UserDto()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    PictureUrl = user.PictureUrl
                };
                result.Add(userDto);
            }
            
            return Ok(result);
        }

        [HttpGet("SearchUsername")]
        public async Task<IActionResult> SearchUser(string searchInput)
        {
            var users = await _userDataAccess.SearchUser(searchInput);
            var result = new List<UserDto>();
            
            foreach (var user in users)
            {
                var userDto = new UserDto()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    PictureUrl = user.PictureUrl
                };
                
                result.Add(userDto);
            }

            return Ok(result);
        }
        
        [HttpPost("uploadPfp")]
        public async Task<IActionResult> UploadPfp(IFormFile file)
        {
            if (file == null || file.Length <= 0) return BadRequest("No file was selected for upload");
            
            var tokenFromRequest = _httpContextAccessor?.HttpContext?.Request.Cookies["X-Access-Token"];
            var userId = _tokenHandler.ReadJwtToken(tokenFromRequest).Claims.First(claim => claim.Type == "userId").Value;

            var pfpUrl = await _r2Service.UploadObject(file);

            var url = await _userDataAccess.UpdateUserPfp(new Guid(userId), pfpUrl);

            return Ok(new { url });

        }
    }
}
