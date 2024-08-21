using MediatR;

namespace Todo.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommand : IRequest
{
    public string UserId { get; init; }
    public string Token { get; init; }

    public ConfirmEmailCommand(string userId, string token)
    {
        UserId = userId;
        Token = token;
    }
}
