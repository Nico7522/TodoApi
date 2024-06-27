using MediatR;

namespace Todo.Application.Team.Commands.AddTask;

public class AssignTaskByTeamCommand : IRequest
{
    public Guid TaskId { get; init; }
    public Guid TeamId { get; init; }

    public AssignTaskByTeamCommand(Guid taskId, Guid teamId)
    {
        TaskId = taskId;
        TeamId = teamId;
    }
}
