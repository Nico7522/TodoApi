using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Todo.Api.Forms.SendMessageForm;
using Todo.Api.Forms.UserStatusForm;
using Todo.Infrastructure.ChatHub;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {

        private readonly IHubContext<Chat, IChat> _hubContext;
        private IUserList _userList;
        public HubController(IHubContext<Chat, IChat> hubContext, IUserList userList)
        {
            _hubContext = hubContext;
            _userList = userList;
        }

        [HttpPost("joinchatroom/{teamId}")]
        public async Task<IActionResult> JoinChatRoom([FromQuery] string connectionId , [FromRoute] string teamId, [FromQuery] string userId)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, teamId);

            _userList.SetOnline(userId);

            var list = _userList.GetList();
            await _hubContext.Clients.Group(teamId).JoinChatRoom(list);
            return NoContent();
        }

        [HttpPost("leftchatroom/{teamId}")]
        public async Task<IActionResult> LeftChatRoom([FromRoute] string teamId, [FromQuery] string userId)
        {
            _userList.SetOffline(userId);

            var list = _userList.GetList();
            await _hubContext.Clients.Group(teamId).LeftChatRoom(list);
            return NoContent();
        }

        [HttpPost("sendmessage/{teamId}")]
        public async Task<IActionResult> SendMessage([FromRoute] string teamId, [FromBody] SendMessageForm form)
        {
            await _hubContext.Clients.Group(teamId).ReceiveMessage(form.Message, form.Firstname, form.Lastname);
            return NoContent();
        }

        [HttpPost("{userId}/setpresence")]
        public async Task<IActionResult> SetPresence([FromRoute] string userId)
        {
            _userList.SetPresence(userId);
            var list = _userList.GetList();
            await _hubContext.Clients.All.SendPresence(list);
            return NoContent();

        }
    }
}
