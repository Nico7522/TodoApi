using MediatR;

namespace Todo.Application.Team.Commands.AddUser;

public class AssignUserByTeamCommand : IRequest
{
    public Guid TeamId { get; init; }

    public string UserId { get; init; }
    public AssignUserByTeamCommand(Guid teamId, string userId)
    {
        TeamId = teamId;
        UserId = userId;
    }
}
