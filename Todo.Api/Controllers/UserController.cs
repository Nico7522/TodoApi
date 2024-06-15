using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Forms.AssignRoleForm;
using Todo.Application.Users.Commands.AssignRole;
using Todo.Application.Users.Commands.AssignTaskByUser;
using Todo.Application.Users.Commands.UnassignRole;
using Todo.Application.Users.Queries.GetTasksByUser;
using Todo.Domain.Constants;
using Todo.Domain.Entities;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPut("{userId}/task/{taskId}")]
        public async Task<ActionResult<TodoEntity?>> AssignTaskByUser([FromRoute] string userId, Guid taskId)
        {
            await _mediator.Send(new AssignTaskByUserCommand(userId, taskId));
            return Ok();
        }

        [HttpGet("{userId}/tasks")]
        public async Task<ActionResult<IEnumerable<TodoEntity>>> GetTasksByUser([FromRoute] string userId)
        {
            var tasks = await _mediator.Send(new GetTasksByUserQuery(userId));
            return Ok(tasks);
        }
        [HttpPut("{userId}/assignrole")]
        public async Task<IActionResult> AssignRole([FromRoute] string userId, [FromBody] AssignRoleForm form)
        {
            AssignRoleCommand command = new AssignRoleCommand(userId, form.Role);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("{userId}/unassignrole")]
        [Authorize(Roles = UserRole.SuperAdmin + "," + UserRole.Admin)]

        public async Task<IActionResult> UnassignRole([FromRoute] string userId)
        {
            UnassignRoleCommand command = new UnassignRoleCommand(userId);
            await _mediator.Send(command);
            return NoContent();
        }

    }
}
