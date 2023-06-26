using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToffApi.DataAccess;
using ToffApi.DtoModels;
using ToffApi.Models;

namespace ToffApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserDataAccess _userDataAccess;
        private readonly IMapper _mapper;

        public UserController(IUserDataAccess userDataAccess, IMapper mapper)
        {
            _userDataAccess = userDataAccess;
            _mapper = mapper;
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
                    Name = user.UserName
                };
                
                result.Add(userDto);
            }

            return Ok(result);
        }
    }
}
