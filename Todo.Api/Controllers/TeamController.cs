using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Team.Dto;
using Todo.Application.Team.GetAllTeams;
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
        public async Task<ActionResult<IEnumerable<TeamDto>>> GetAll([FromQuery] GetAllTeamQuery query)
        {
            var teams = await _mediator.Send(query);
            return Ok(teams);
        }
    }
}
