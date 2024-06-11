using MediatR;
using Todo.Domain.Repositories;
using Todo.Domain.Security;

namespace Todo.Application.Users.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IAuthRepository _authRepository;
    public LoginCommandHandler(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }
    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {

        return await _authRepository.Login(request.Email, request.Password);
    }
}
