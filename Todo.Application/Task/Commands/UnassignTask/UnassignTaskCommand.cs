using MediatR;

namespace Todo.Application.Task.Commands.UnassignTask;

public class UnassignTaskCommand : IRequest
{
    public Guid TaskId { get; init; }
    public UnassignTaskCommand(Guid taskId)
    {
        TaskId = taskId;
    }
}
