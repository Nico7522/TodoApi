using MediatR;

namespace Todo.Application.Team.Commands.CloseTask;

public class CloseTeamTaskCommand : IRequest
{
    public Guid TeamId { get; init; }

    public Guid TaskId { get; init; }

    public CloseTeamTaskCommand(Guid teamId, Guid taskId)
    {
        TeamId = teamId;
        TaskId = taskId;
    }
}
