using MediatR;

namespace Todo.Application.Team.Commands.UnassignLeader;

public class UnassignLeaderByTeamCommand : IRequest
{
    public Guid TeamId { get; init; }
    public UnassignLeaderByTeamCommand(Guid teamId)
    {
        TeamId = teamId;
    }
}
