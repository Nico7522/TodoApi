using MediatR;

namespace Todo.Application.Team.Commands.CloseTask;

public class CloseTaskCommand : IRequest
{
    public Guid TeamId { get; init; }

    public Guid TaskId { get; init; }

    public CloseTaskCommand(Guid teamId, Guid taskId)
    {
        TeamId = teamId;
        TaskId = taskId;
    }
}
