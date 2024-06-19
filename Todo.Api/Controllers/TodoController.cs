using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todo.Api.Forms.CompleteTaskForm;
using Todo.Api.Forms.UpdateTaskForm;
using Todo.Application.Task.Commands.CompleteTask;
using Todo.Application.Task.Commands.CreateTask;
using Todo.Application.Task.Commands.DeleteTask;
using Todo.Application.Task.Commands.UpdateTask;
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
        public async Task<ActionResult<TodoDto?>> GetTaskById(Guid taskId)
        {
            var task = await _mediator.Send(new GetTaskByIdQuery(taskId));
            return Ok(task);
        }


        [HttpPut("{taskId}/complete")]
        public async Task<IActionResult> CompleteTask([FromRoute] Guid taskId, CompleteTaskForm form)
        {
            CompleteTaskCommand command = new CompleteTaskCommand(taskId, form.Duration);
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPut("{taskId}")]
        public async Task<IActionResult> UpdateTask([FromRoute] Guid taskId, UpdateTaskForm form)
        {
            UpdateTaskCommand command = new UpdateTaskCommand();
            command.Id = taskId;
            command.Description = form.Description;
            command.Title = form.Title;
            command.Priority = form.Priority;
            await _mediator.Send(command);
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
    }
}
