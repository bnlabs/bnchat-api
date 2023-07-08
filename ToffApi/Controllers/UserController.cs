using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Internal;
using ToffApi.Command.CommandBuses;
using ToffApi.Command.CommandHandlers;
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
        private readonly UserQueryHandler _userQueryHandler;
        private readonly UserCommandHandler _userCommandHandler;

        public UserController(JwtSecurityTokenHandler tokenHandler,
            IHttpContextAccessor httpContextAccessor,
            UserQueryHandler userQueryHandler, 
            UserCommandHandler userCommandHandler)
            : base(tokenHandler, httpContextAccessor)
        {
            _userQueryHandler = userQueryHandler;
            _userCommandHandler = userCommandHandler;
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
            var query = new GetUsersByIdsQuery() { ListOfUserIds = listOfUserId };
            var queryResult = await _userQueryHandler.HandleAsync(query);
            return Ok(queryResult.ListOfUsers);
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
            var userId = new Guid(ExtractUserId());
            var command = new UpdatePfpCommand() { UserId = userId, File = file };
            var commandResult = await _userCommandHandler.HandleAsync(command);
            
            return Ok(commandResult.Url);

        }
    }
}
