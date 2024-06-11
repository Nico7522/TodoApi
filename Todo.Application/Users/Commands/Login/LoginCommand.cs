using MediatR;

namespace Todo.Application.Users.Commands.Login;

public class LoginCommand : IRequest<string>
{
    public string Email { get; set; } = default!; 
    public string Password { get; set; } = default!;

}
