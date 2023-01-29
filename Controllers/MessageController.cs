using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRChatApp.Hubs;

namespace SignalRChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        protected readonly IHubContext<MessageHub> _hubContext;

        public MessageController(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] MessagePost messagePost)
        {
            
            await _hubContext.Clients.Group(messagePost.ConversationName).SendAsync("ReceiveMessage", messagePost);

            return Ok();
        }
    }

    public class MessagePost
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public string ConversationName { get; set; }
    }
}