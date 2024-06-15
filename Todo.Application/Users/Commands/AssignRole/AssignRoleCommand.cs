using MediatR;
using Todo.Domain.Constants;

namespace Todo.Application.Users.Commands.AssignRole;

public class AssignRoleCommand : IRequest
{
    public string UserId { get; init; }
    public string Role { get; init; }

    public AssignRoleCommand(string userId, string role)
    {
        UserId = userId;
        Role = role;
    }
}
