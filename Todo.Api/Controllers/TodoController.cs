using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Application.Task.Commands.CompleteTask;
using Todo.Application.Task.Dto;
using Todo.Application.Task.Queries.GetTaskById;
using Todo.Application.Users.Commands.AssignTaskByUser;
using Todo.Application.Users.Queries.GetTasksByUser;
using Todo.Domain.Entities;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TodoController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{taskId}")]
        public async Task<ActionResult<TodoDto?>> GetTaskById(string taskId)
        {
            var task = await _mediator.Send(new GetTaskByIdQuery(taskId));
            return Ok(task);
        }

        [HttpPut("{taskId}/user/{userId}")]
        public async Task<ActionResult<TodoEntity?>> AssignTaskByUser([FromRoute] string userId,string taskId)
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

        [HttpPut("{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(string taskId)
        {
            await _mediator.Send(new CompleteTaskCommand(taskId));
            return Ok();
        }
    }
}
