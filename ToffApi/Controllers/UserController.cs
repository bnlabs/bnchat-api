using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToffApi.DtoModels;
using ToffApi.Exceptions;
using ToffApi.Query.Queries;
using ToffApi.Query.QueryHandlers;
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
        private readonly UserQueryHandler _userQueryHandler;

        public UserController(JwtSecurityTokenHandler tokenHandler,
            IHttpContextAccessor httpContextAccessor,
            IUserDataAccess userDataAccess,
            IR2Service r2Service, 
            UserQueryHandler userQueryHandler)
            : base(tokenHandler, httpContextAccessor)
        {
            _userDataAccess = userDataAccess;
            _r2Service = r2Service;
            _userQueryHandler = userQueryHandler;
        }

        [HttpGet("getUserById")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
            {
                var query = new GetUserByIdQuery() { UserId = userId };
                var queryResult = await _userQueryHandler.HandleAsync(query);
                return Ok(queryResult.User);
            }
            catch(UserNotFoundException)
            {
                return NotFound("user not found");
            }
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
            var query = new SearchUserByUsernameQuery() { searchQuery = searchInput };
            var queryResult = await _userQueryHandler.HandleAsync(query);
           return Ok(queryResult.ListOfUsers);
        }
        
        [HttpPost("uploadPfp")]
        public async Task<IActionResult> UploadPfp(IFormFile file)
        {
            if (file is not { Length: > 0 }) return BadRequest("No file was selected for upload");
            var userId = ExtractUserId();
            var pfpUrl = await _r2Service.UploadObject(file);
            var url = await _userDataAccess.UpdateUserPfp(new Guid(userId), pfpUrl);
            return Ok(new { url });

        }
    }
}
