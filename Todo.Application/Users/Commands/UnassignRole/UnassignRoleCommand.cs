using MediatR;
namespace Todo.Application.Users.Commands.UnassignRole;

public class UnassignRoleCommand : IRequest
{
    public string UserId { get; set; } = default!;

    public UnassignRoleCommand(string userId)
    {
        UserId = userId;
    }
}
