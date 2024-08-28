using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Forms.AssignRoleForm;
using Todo.Api.Forms.ResetPasswordConfirmForm;
using Todo.Application.Task.Dto;
using Todo.Application.Team.Dto;
using Todo.Application.Users.Commands.AssignRole;
using Todo.Application.Users.Commands.AssignTaskByUser;
using Todo.Application.Users.Commands.DeleteUser;
using Todo.Application.Users.Commands.ResetPassword;
using Todo.Application.Users.Commands.ResetPasswordConfirm;
using Todo.Application.Users.Commands.UnassignRole;
using Todo.Application.Users.Dto;
using Todo.Application.Users.Queries.GetTasksNumberByUser;
using Todo.Application.Users.Queries.GetTeamByUser;
using Todo.Application.Users.Queries.GetUserById;
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

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] string userId)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(userId));
            return Ok(user);
        }

        [HttpGet("{userId}/team")]
        public async Task<ActionResult<TeamDto>> GetTeamByUser([FromRoute] string userId)
        {
            var team = await _mediator.Send(new GetTeamByUserQuery(userId));
            return Ok(team);
        }


        [HttpPost("{userId}/task/{taskId}")]
        public async Task<ActionResult<TodoEntity?>> AssignTaskByUser([FromRoute] string userId, Guid taskId)
        {
            await _mediator.Send(new AssignTaskByUserCommand(userId, taskId));
            return Ok();
        }
 
        [HttpPut("{userId}/assignrole")]
        //[Authorize(Roles = UserRole.SuperAdmin + "," + UserRole.Admin)]
        public async Task<IActionResult> AssignRole([FromRoute] string userId, [FromBody] AssignRoleForm form)
        {
            await _mediator.Send(new AssignRoleCommand(userId, form.Role));
            return NoContent();
        }

        [HttpPut("{userId}/unassignrole")]
        [Authorize(Roles = UserRole.SuperAdmin + "," + UserRole.Admin)]
        public async Task<IActionResult> UnassignRole([FromRoute] string userId)
        {
            await _mediator.Send(new UnassignRoleCommand(userId));
            return NoContent();
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            await _mediator.Send(command);  
            return NoContent();
        }

        [HttpPost("{userId}/{resetToken}/resetpasswordconfirm")]
        public async Task<IActionResult> ResetPasswordConfirm(string userId, string resetToken, [FromBody] ResetPasswordConfirmForm form)
        {
            await _mediator.Send(new ResetPasswordConfirmCommand(userId, resetToken, form.Password, form.PasswordConfirm));
            return NoContent();
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete([FromRoute] string userId)
        {
            await _mediator.Send(new DeleteUserCommand(userId));
            return NoContent();
        }

        [HttpGet("{userId}/tasksnumber")]
        public async Task<IActionResult> GetTasksNumberByUser([FromRoute] string userId)
        {
            var tasksNumber = await _mediator.Send(new GetTasksNumberByUserQuery(userId));
            return Ok(tasksNumber);
        }

   


    }
}
