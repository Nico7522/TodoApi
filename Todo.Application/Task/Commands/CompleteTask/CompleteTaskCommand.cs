using MediatR;

namespace Todo.Application.Task.Commands.CompleteTask;

public class CompleteTaskCommand : IRequest<bool>
{
    public string TaskId { get; init; }
    public CompleteTaskCommand(string taskId)
    {
        TaskId = taskId;
    }
}
