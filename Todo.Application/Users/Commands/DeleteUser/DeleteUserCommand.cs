using MediatR;

namespace Todo.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest
{
    public string UserId { get; init; }
    public DeleteUserCommand(string userId)
    {
        UserId = userId;
    }
}
