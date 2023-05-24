using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToffApi.DataAccess;
using ToffApi.DtoModels;
using ToffApi.Models;

namespace ToffApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageDataAccess _messageDataAccess;

        public MessageController(IMessageDataAccess messageDataAccess)
        {
            _messageDataAccess = messageDataAccess;
        }
        
        [HttpGet("getConversation")]
        public async Task<IActionResult> GetConversation(Guid userId, Guid conversationId)
        {
            var result = await _messageDataAccess.GetMessagesFromConversation(userId, conversationId);
            return Ok(result);
        }
        
        [HttpPost("add")]
        public async Task<IActionResult> AddMessage(MessageDto msg)
        {
            var messageMapped = new Message
            {
                SenderId = msg.SenderId,
                Content = msg.Content,
                ConversationId = msg.ConversationId,
                TimeStamp = DateTime.Now
            };
            
            await _messageDataAccess.AddMessage(messageMapped);
            return Ok();
        }
        
    }
}
