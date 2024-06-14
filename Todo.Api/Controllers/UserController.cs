﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Users.Commands.AssignTaskByUser;
using Todo.Application.Users.Queries.GetTasksByUser;
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
        public async Task<ActionResult<TodoEntity?>> AssignTaskByUser([FromRoute] string userId, string taskId)
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

    }
}
