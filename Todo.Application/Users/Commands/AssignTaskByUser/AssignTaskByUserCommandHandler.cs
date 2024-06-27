using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Users.Commands.AssignTaskByUser;

internal class AssignTaskByUserCommandHandler : IRequestHandler<AssignTaskByUserCommand>
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly ITodoRepository _todoRepository;

    public AssignTaskByUserCommandHandler(UserManager<UserEntity> userMananger, ITodoRepository todoRepository )
    { 
        _userManager = userMananger;
        _todoRepository = todoRepository;
    }

    public async System.Threading.Tasks.Task Handle(AssignTaskByUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        if (task.UserId == user.Id) throw new BadRequestException("Task already assigned to this user");

        if (task.UserId != null || task.TeamId != null) throw new BadRequestException("Task is already assigned");

        user.Tasks.Add(task);
        await _todoRepository.SaveChanges();
    }
}
