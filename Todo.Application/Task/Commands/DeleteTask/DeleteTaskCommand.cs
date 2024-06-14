
using MediatR;

namespace Todo.Application.Task.Commands.DeleteTask;

public class DeleteTaskCommand : IRequest
{
    public string TaskId { get; init; }
    public DeleteTaskCommand(string taskId)
    {
        TaskId = taskId;
    }
}
