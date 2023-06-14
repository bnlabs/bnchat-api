using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToffApi.AuthenticationService;
using ToffApi.DataAccess;
using ToffApi.DtoModels;
using ToffApi.Models;

namespace ToffApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageDataAccess _messageDataAccess;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public MessageController(IMessageDataAccess messageDataAccess,
            IHttpContextAccessor httpContextAccessor, JwtSecurityTokenHandler tokenHandler)
        {
            _messageDataAccess = messageDataAccess;
            _httpContextAccessor = httpContextAccessor;
            _tokenHandler = tokenHandler;
        }
        
        [HttpGet("getConversation")]
        public async Task<IActionResult> GetConversationByUserId(Guid userId)
        {
            var conversations = await _messageDataAccess.GetConversationByUserId(userId);
            return Ok(conversations);
        }

        [HttpGet("getConversationById")]
        public async Task<IActionResult> GetConversationById(Guid conversationId)
        {
            var result = await _messageDataAccess.GetConversationById(conversationId);
            return Ok(result[0]);
        }
        
        [HttpPost("createConversation")]
        public async Task<IActionResult> CreateConversation(ConversationDto conversationDto)
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                throw new UnauthorizedAccessException();
            }
        
            var tokenFromRequest = _httpContextAccessor.HttpContext.Request.Cookies["X-Access-Token"];
            if (string.IsNullOrEmpty(tokenFromRequest))
            {
                throw new UnauthorizedAccessException();
            }
        
            var userId = _tokenHandler.ReadJwtToken(tokenFromRequest).Claims.First(claim => claim.Type == "userId").Value;
            conversationDto.MemberIds.Add(new Guid(userId));
            conversationDto.MemberIds = conversationDto.MemberIds.Distinct().ToList();
            var c = new Conversation(conversationDto.MemberIds);
            await _messageDataAccess.AddConversation(c);
            return Ok();
        }

    }
}
