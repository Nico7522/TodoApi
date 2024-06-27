using MediatR;

namespace Todo.Application.Team.Commands.CloseTask;

public class CompleteTaskByTeamCommand : IRequest
{
    public Guid TeamId { get; init; }

    public Guid TaskId { get; init; }

    public CompleteTaskByTeamCommand(Guid teamId, Guid taskId)
    {
        TeamId = teamId;
        TaskId = taskId;
    }
}
