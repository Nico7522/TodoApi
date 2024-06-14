using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Forms;
using Todo.Application.Task.Commands.CompleteTask;
using Todo.Application.Task.Dto;
using Todo.Application.Task.Queries.GetTaskById;

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


        [HttpPut("{taskId}/complete")]
        public async Task<IActionResult> CompleteTask([FromRoute]string taskId, CompleteTaskForm form)
        {
            CompleteTaskCommand command = new CompleteTaskCommand(taskId, form.Duration);
            await _mediator.Send(command);
            return Ok();
        }
    }
}
