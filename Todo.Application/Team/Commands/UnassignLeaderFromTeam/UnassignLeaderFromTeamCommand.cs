using MediatR;

namespace Todo.Application.Team.Commands.UnassignLeader;

public class UnassignLeaderFromTeamCommand : IRequest
{
    public Guid TeamId { get; init; }
    public UnassignLeaderFromTeamCommand(Guid teamId)
    {
        TeamId = teamId;
    }
}
