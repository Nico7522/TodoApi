using MediatR;

namespace Todo.Application.Team.Commands.UnassignLeader;

public class UnassignLeaderCommand : IRequest
{
    public Guid TeamId { get; init; }
    public UnassignLeaderCommand(Guid teamId)
    {
        TeamId = teamId;
    }
}
