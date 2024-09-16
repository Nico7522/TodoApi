using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Forms.UpdateTaskByTeamForm;
using Todo.Application.Team.Commands.AddTask;
using Todo.Application.Team.Commands.AddUser;
using Todo.Application.Team.Commands.AssignLeader;
using Todo.Application.Team.Commands.CloseTask;
using Todo.Application.Team.Commands.CloseTeam;
using Todo.Application.Team.Commands.CreateTeam;
using Todo.Application.Team.Commands.DeleteUser;
using Todo.Application.Team.Commands.UnassignLeader;
using Todo.Application.Team.Commands.UnassignTaskFromTeam;
using Todo.Application.Team.Commands.UpdateTeamTask;
using Todo.Application.Team.Dto;
using Todo.Application.Team.Queries.GetAllTeams;
using Todo.Application.Team.Queries.GetTeamById;
using Todo.Domain.Constants;


namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TeamController(IMediator meditor)
        {
            _mediator = meditor;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAll([FromQuery] bool isActive)
        {
            var teams = await _mediator.Send(new GetAllTeamQuery(isActive));
            return Ok(teams);
        }


        [HttpGet("{teamId}")]
        public async Task<ActionResult<TeamDto?>> GetTeamById([FromRoute] Guid teamId)
        {
            var team = await _mediator.Send(new GetTeamByIdQuery(teamId));
            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeamCommand command)
        {
            var teamId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTeamById), new { teamId }, null);
        }

        [HttpPut("{teamId}/close")]
        [Authorize(Roles = UserRole.SuperAdmin)]
        public async Task<IActionResult> CloseTeam([FromRoute] Guid teamId)
        {
            await _mediator.Send(new CloseTeamCommand(teamId));
            return Ok();
        }

        [HttpPost("{teamId}/user/{userId}")]
        public async Task<IActionResult> AssignUserByTeam([FromRoute] Guid teamId, [FromRoute] string userId)
        {
            await _mediator.Send(new AssignUserByTeamCommand(teamId, userId));
            return Ok();
        }

        [HttpDelete("{teamId}/user/{userId}")]
        public async Task<IActionResult> DeleteUserFromTeam([FromRoute] Guid teamId, [FromRoute] string userId)
        {
            await _mediator.Send(new UnassignUserFromTeamCommand(teamId, userId));
            return Ok();
        }

        [HttpPut("{teamId}/user/{userId}/assignleader")]
        [Authorize(Roles = UserRole.SuperAdmin)]
        public async Task<IActionResult> AssignLeaderByTeam([FromRoute] Guid teamId, [FromRoute] string userId)
        {
            await _mediator.Send(new AssignLeaderByTeamCommand(teamId, userId));
            return Ok();
        }

        [HttpPut("{teamId}/unassignleader")]
        public async Task<IActionResult> UnassignLeaderFromTeam([FromRoute] Guid teamId)
        {
            await _mediator.Send(new UnassignLeaderFromTeamCommand(teamId));
            return Ok();
        }

        [HttpPost("{teamId}/task/{taskId}")]
        public async Task<IActionResult> AddTaskToTeam([FromRoute] Guid teamId, [FromRoute] Guid taskId)
        {
            await _mediator.Send(new AssignTaskByTeamCommand(taskId, teamId));
            return Ok();
        }

        [HttpPut("{teamId}/task/{taskId}")]
        public async Task<IActionResult> UpdateTaskByTeam([FromRoute] Guid taskId, [FromRoute] Guid teamId, [FromBody] UpdateTaskByTeamForm form)
        {
            await _mediator.Send(new UpdateTaskByTeamCommand(taskId, teamId, form.Title, form.Description, form.Priority));
            return Ok();
        }

        [HttpPut("{teamId}/task/{taskId}/complete")]
        public async Task<IActionResult> CompleteTaskByTeam([FromRoute] Guid teamId, [FromRoute] Guid taskId)
        {
            await _mediator.Send(new CompleteTaskByTeamCommand(teamId, taskId));
            return Ok();
        }

        [HttpDelete("{teamId}/task/{taskId}")]
        public async Task<IActionResult> UnassignTaskFromTeam([FromRoute] Guid teamId, [FromRoute,] Guid taskId)
        {
            await _mediator.Send(new UnassignTaskFromTeamCommand(teamId, taskId));
            return NoContent();
        }
 

    }
}
