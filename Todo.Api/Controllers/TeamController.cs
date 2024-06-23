using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Todo.Application.Team.Commands.AddUser;
using Todo.Application.Team.Commands.CloseTeam;
using Todo.Application.Team.Commands.CreateTeam;
using Todo.Application.Team.Dto;
using Todo.Application.Team.Queries.GetAllTeams;
using Todo.Application.Team.Queries.GetTeamById;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;

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
        public async Task<IActionResult> CloseTeam([FromRoute] Guid teamId)
        {
            await _mediator.Send(new CloseTeamCommand(teamId));
            return Ok();
        }

        [HttpPost("{teamId}/user/{userId}")]
        public async Task<IActionResult> AddUser([FromRoute] Guid teamId, [FromRoute] string userId)
        {
            await _mediator.Send(new AddUserCommand(teamId, userId));
            return Ok();
        }
    }
}
