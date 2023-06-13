using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public MessageController(IMessageDataAccess messageDataAccess)
        {
            _messageDataAccess = messageDataAccess;
        }
        
        [HttpGet("getConversation")]
        public async Task<IActionResult> GetConversationByUserId(Guid userId)
        {
            var conversations = await _messageDataAccess.GetConversationByUserId(userId);
            foreach (var c in conversations)
            {
                c.Messages = await _messageDataAccess.GetMessagesFromConversation(userId, c.ConversationId);
            }
            return Ok(conversations);
        }
        
        // This should be a signalR hub method
        // [HttpPost("addMessage")]
        // public async Task<IActionResult> AddMessage(MessageDto msg)
        // {
        //     var messageMapped = new Message
        //     {
        //         SenderName = msg.SenderName,
        //         SenderId = msg.SenderId,
        //         Content = msg.Content,
        //         ConversationId = msg.ConversationId,
        //         Timestamp = DateTime.Now
        //     };
        //     
        //     await _messageDataAccess.AddMessage(messageMapped);
        //     return Ok();
        // }
        
        [HttpPost("createConversation")]
        public async Task<IActionResult> CreateConversation(ConversationDto conversationDto)
        {
            var c = new Conversation(conversationDto.MemberIds);
            await _messageDataAccess.AddConversation(c);
            return Ok();
        }

    }
}
