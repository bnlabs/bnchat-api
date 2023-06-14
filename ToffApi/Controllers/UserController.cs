using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToffApi.DataAccess;
using ToffApi.DtoModels;

namespace ToffApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDataAccess _userDataAccess;

        public UserController(IUserDataAccess userDataAccess)
        {
            _userDataAccess = userDataAccess;
        }

        [HttpGet("getUserById")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var users = await _userDataAccess.GetUserByIdAsync(userId);
            var resultUser = new UserDto()
            {
                Name = users[0].UserName,
                Id = users[0].Id
            };
            return Ok(resultUser);
        }
    }
}
