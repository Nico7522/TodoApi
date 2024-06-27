
using MediatR;

namespace Todo.Application.Team.Commands.AssignLeader;

public class AssignLeaderByTeamCommand : IRequest
{
    public Guid TeamId { get; init; }
    public string UserId { get; init; }
    public AssignLeaderByTeamCommand(Guid teamId, string userId)
    {
        TeamId = teamId;
        UserId = userId;
    }
}
