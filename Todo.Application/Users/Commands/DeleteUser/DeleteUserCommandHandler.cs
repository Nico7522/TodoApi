using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;

namespace Todo.Application.Users.Commands.DeleteUser;

internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly UserManager<UserEntity> _userManager;
    public DeleteUserCommandHandler(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }
    public async System.Threading.Tasks.Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        var result = await _userManager.DeleteAsync(user);
        if(result.Succeeded)
        {
            await Console.Out.WriteLineAsync("cc");
            await Console.Out.WriteLineAsync(result.Errors.First().ToString());

        }
    }
}
