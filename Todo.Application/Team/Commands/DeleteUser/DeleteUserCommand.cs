
using MediatR;

namespace Todo.Application.Team.Commands.DeleteUser;

public class DeleteUserCommand : IRequest
{
    public Guid TeamId { get; init; }
    public string UserId { get; init; }
    public DeleteUserCommand(Guid teamId, string userId)
    {
        TeamId = teamId;
        UserId = userId;
    }
}
