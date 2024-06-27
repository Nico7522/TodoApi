
using MediatR;

namespace Todo.Application.Task.Commands.CompleteTask;

public class CompleteTaskCommand : IRequest
{
    public Guid TaskId { get; init; }
    public int Duration { get; init; }

    public CompleteTaskCommand(Guid taskId, int duration)
    {
        TaskId = taskId;
        Duration = duration;
    }
}
