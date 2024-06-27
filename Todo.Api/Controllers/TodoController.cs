using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Forms.CompleteTaskForm;
using Todo.Api.Forms.UpdateTaskForm;
using Todo.Application.Task.Commands.CompleteTask;
using Todo.Application.Task.Commands.CreateTask;
using Todo.Application.Task.Commands.DeleteTask;
using Todo.Application.Task.Commands.UnassignTask;
using Todo.Application.Task.Commands.UpdateTask;
using Todo.Application.Task.Dto;
using Todo.Application.Task.Queries.GetAllActiveTasks;
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
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetAllActive()
        {
            var task = await _mediator.Send(new GetAllActiveTasksQuery());
            return Ok(task);
        }

        [HttpGet("{taskId}")]
        public async Task<ActionResult<TodoDto?>> GetTaskById(Guid taskId)
        {
            var task = await _mediator.Send(new GetTaskByIdQuery(taskId));
            return Ok(task);
        }


        [HttpPut("{taskId}/complete")]
        public async Task<IActionResult> CompleteTask([FromRoute] Guid taskId, CompleteTaskForm form)
        {
            await _mediator.Send(new CompleteTaskCommand(taskId, form.Duration));
            return Ok();
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid taskId, UpdateTaskForm form)
        {
           
            await _mediator.Send(new UpdateTaskCommand(taskId, form.Title, form.Description, form.Priority));
            return NoContent();
        }

        [HttpDelete("{taskId}")]
        public async Task<IActionResult> DeleteTask([FromRoute] Guid taskId)
        {
            await _mediator.Send(new DeleteTaskCommand(taskId));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskCommand command)
        {
            var taskId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTaskById), new { taskId }, null);
        }

        [HttpPut("{taskId}/unassign")]
        public async Task<IActionResult> UnassignTask([FromRoute] Guid taskId)
        {
            await _mediator.Send(new UnassignTaskCommand(taskId));
            return NoContent();
        }

    }
}
