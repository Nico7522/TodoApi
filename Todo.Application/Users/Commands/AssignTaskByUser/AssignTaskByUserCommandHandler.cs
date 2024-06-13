using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Users.Commands.AssignTaskByUser;

internal class AssignTaskByUserCommandHandler : IRequestHandler<AssignTaskByUserCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<UserEntity> _userManager;

    public AssignTaskByUserCommandHandler(IUserRepository userRepository, UserManager<UserEntity> userMananger )
    { 
        _userRepository = userRepository;
        _userManager = userMananger;
    }
    public async Task<bool> Handle(AssignTaskByUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new ApiErrorException("User not found", 404);

        // TODO: Add GetTaskBuId
        //var task = await 
    }
}
