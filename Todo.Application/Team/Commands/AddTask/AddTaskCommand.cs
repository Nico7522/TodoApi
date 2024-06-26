using MediatR;

namespace Todo.Application.Team.Commands.AddTask;

public class AddTaskCommand : IRequest
{
    public Guid TaskId { get; init; }
    public Guid TeamId { get; init; }

    public AddTaskCommand(Guid taskId, Guid teamId)
    {
        TaskId = taskId;
        TeamId = teamId;
    }
}
