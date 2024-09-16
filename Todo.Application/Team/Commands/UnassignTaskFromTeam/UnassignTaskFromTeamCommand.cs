using MediatR;

namespace Todo.Application.Team.Commands.UnassignTaskFromTeam;

public class UnassignTaskFromTeamCommand : IRequest
{
    public Guid TeamId { get; set; }
    public Guid TaskId { get; set; }

    public UnassignTaskFromTeamCommand(Guid teamId, Guid taskId)
    {
        TeamId = teamId;
        TaskId = taskId;
    }
}
