using MediatR;

namespace Todo.Application.Team.Commands.AddUser;

public class AddUserCommand : IRequest
{
    public Guid TeamId { get; init; }

    public string UserId { get; init; }
    public AddUserCommand(Guid teamId, string userId)
    {
        TeamId = teamId;
        UserId = userId;
    }
}
