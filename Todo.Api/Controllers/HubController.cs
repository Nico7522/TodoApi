using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Infrastructure.ChatHub;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {

        private readonly IHubContext<Chat> _hubContext;

        public HubController(IHubContext<Chat> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> JoinChatRoom(string teamId)
        {
            return NoContent();
        }
    }
}
