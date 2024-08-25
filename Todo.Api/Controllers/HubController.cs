using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Todo.Api.Forms.SendMessageForm;
using Todo.Infrastructure.ChatHub;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {

        private readonly IHubContext<Chat, IChat> _hubContext;

        public HubController(IHubContext<Chat, IChat> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("joinchatroom/{teamId}")]
        public async Task<IActionResult> JoinChatRoom([FromQuery] string connectionId , [FromRoute] string teamId, [FromQuery] string firstname, [FromQuery] string lastname)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, teamId);
            await _hubContext.Clients.Group(teamId).JoinChatRoom($"{firstname} {lastname} has joined chat");
            return NoContent();
        }

        [HttpPost("sendmessage/{teamId}")]
        public async Task<IActionResult> SendMessage([FromRoute] string teamId, [FromBody] SendMessageForm form)
        {
            await _hubContext.Clients.Group(teamId).ReceiveMessage(form.Message, form.Firstname, form.Lastname);
            return NoContent();
        }
    }
}
