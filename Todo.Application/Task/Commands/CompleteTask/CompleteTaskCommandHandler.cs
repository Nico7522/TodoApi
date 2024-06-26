using MediatR;
using Todo.Application.Users;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.CompleteTask;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUserContext _userContext;

    private readonly IAuthorization<TodoEntity> _authorization;
    public CompleteTaskCommandHandler(ITodoRepository todoRepository, IAuthorization<TodoEntity> authorization, IUserContext userContext)
    {
        _todoRepository = todoRepository;
        _authorization = authorization;
        _userContext = userContext;
    }
    public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {

        var currentUser = _userContext.GetCurrentUser();

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        if (currentUser!.Role == UserRole.User || currentUser.Role == UserRole.Leader)
        {
            if (currentUser.Id != task.UserId) throw new ForbidException("Your not authorized");
        }


        var time = new TimeOnly().AddMinutes(request.Duration);

        task.IsComplete = true;
        task.Duration = time;
        await _todoRepository.SaveChanges();

        return true;
    }
}
