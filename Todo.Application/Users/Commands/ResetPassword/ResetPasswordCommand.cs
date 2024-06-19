
using MediatR;

namespace Todo.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest
{
    public string Email { get; init; }
    public ResetPasswordCommand(string email)
    {
        Email = email;
    }
}
