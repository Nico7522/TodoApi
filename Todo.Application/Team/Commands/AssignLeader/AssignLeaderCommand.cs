
using MediatR;

namespace Todo.Application.Team.Commands.AssignLeader;

public class AssignLeaderCommand : IRequest
{
    public Guid TeamId { get; init; }
    public string UserId { get; init; }
    public AssignLeaderCommand(Guid teamId, string userId)
    {
        TeamId = teamId;
        UserId = userId;
    }
}
