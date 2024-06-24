using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Users.Commands.UnassignTaskByUser;

internal class UnassignTaskByUserCommandHandler : IRequestHandler<UnassignTaskByUserCommand>
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly ITodoRepository _todoRepository;

    public UnassignTaskByUserCommandHandler(UserManager<UserEntity> userManager, ITodoRepository todoRepository)
    {
        _userManager = userManager;
        _todoRepository = todoRepository;   
    }
    public async System.Threading.Tasks.Task Handle(UnassignTaskByUserCommand request, CancellationToken cancellationToken)
    {

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        task.User = null;
        await _todoRepository.SaveChanges();
    }
}
