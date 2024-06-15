
using MediatR;

namespace Todo.Application.Task.Commands.DeleteTask;

public class DeleteTaskCommand : IRequest
{
    public Guid TaskId { get; init; }
    public DeleteTaskCommand(Guid taskId)
    {
        TaskId = taskId;
    }
}
