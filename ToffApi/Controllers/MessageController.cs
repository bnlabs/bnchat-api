using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToffApi.DtoModels;
using ToffApi.Models;
using ToffApi.Query.Queries;
using ToffApi.Query.QueryHandlers;
using ToffApi.Query.QueryResults;
using ToffApi.Services.DataAccess;

namespace ToffApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly IMessageDataAccess _messageDataAccess;
        private readonly IUserDataAccess _userDataAccess;
        private readonly MessageQueryHandler _messageQueryHandler;

        public MessageController(JwtSecurityTokenHandler tokenHandler,
            IHttpContextAccessor httpContextAccessor,
            IMessageDataAccess messageDataAccess,
            IUserDataAccess userDataAccess, 
            MessageQueryHandler messageQueryHandler) : base(tokenHandler, httpContextAccessor)
        {
            _messageDataAccess = messageDataAccess;
            _userDataAccess = userDataAccess;
            _messageQueryHandler = messageQueryHandler;
        }
        
        [HttpGet("getConversation")]
        public async Task<IActionResult> GetConversationByUserId()
        {
            var userId = new Guid(ExtractUserId());
            var query = new GetConversationsByUserIdQuery(userId);
            var queryResult = await _messageQueryHandler.HandleAsync(query);
            return Ok(queryResult.ConversationList);
        }
        
        [HttpGet("getConversationById")]
        public async Task<IActionResult> GetConversationById(Guid conversationId)
        {
            var query = new GetConversationByIdQuery(conversationId);
            var queryResult = await _messageQueryHandler.HandleAsync(query);
            return Ok(queryResult);
        }
        
    }
}
