
using MediatR;

namespace Todo.Application.Team.Commands.DeleteUser;

public class UnassignUserFromTeamCommand : IRequest
{
    public Guid TeamId { get; init; }
    public string UserId { get; init; }
    public UnassignUserFromTeamCommand(Guid teamId, string userId)
    {
        TeamId = teamId;
        UserId = userId;
    }
}
