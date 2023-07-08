using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToffApi.DtoModels;
using ToffApi.Models;
using ToffApi.Services.DataAccess;

namespace ToffApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class MessageController : Controller
    {
        private readonly IMessageDataAccess _messageDataAccess;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IUserDataAccess _userDataAccess;

        public MessageController(IMessageDataAccess messageDataAccess,
            IHttpContextAccessor httpContextAccessor, JwtSecurityTokenHandler tokenHandler, IUserDataAccess userDataAccess)
        {
            _messageDataAccess = messageDataAccess;
            _httpContextAccessor = httpContextAccessor;
            _tokenHandler = tokenHandler;
            _userDataAccess = userDataAccess;
        }
        
        // there is a handleAsync
        [HttpGet("getConversation")]
        public async Task<IActionResult> GetConversationByUserId(Guid userId)
        {
            var conversations = await _messageDataAccess.GetConversationByUserId(userId);
            var conversationResultList = conversations.Select(c => new ConversationDto
            {
                ConversationId = c.ConversationId,
                MemberIds = c.MemberIds
            }).ToList();

            foreach (var c in conversationResultList)
            {
                var memberMap = c.MemberIds.ToDictionary(
                    id => id,
                    id => _userDataAccess.GetUserById(id)[0].UserName
                );
                c.MemberMap = memberMap;
                c.Messages = await _messageDataAccess.GetMessagesFromConversation(userId, c.ConversationId);
                c.Messages = c.Messages.OrderByDescending(m => m.Timestamp).ToList();
            }
            return Ok(conversationResultList);
        }

        // there is a handleAsync
        [HttpGet("getConversationById")]
        public async Task<IActionResult> GetConversationById(Guid conversationId)
        {
            var result = await _messageDataAccess.GetConversationById(conversationId);
            return Ok(result[0]);
        }
        
    }
}
