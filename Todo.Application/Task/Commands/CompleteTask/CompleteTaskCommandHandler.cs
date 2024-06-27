using MediatR;
using Todo.Application.Users;
using Todo.Domain.Constants;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.CompleteTask;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IUserContext _userContext;
    public CompleteTaskCommandHandler(ITodoRepository todoRepository, IUserContext userContext)
    {
        _todoRepository = todoRepository;
        _userContext = userContext;
    }
    public async System.Threading.Tasks.Task Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        if (currentUser!.Role == UserRole.User || currentUser.Role == UserRole.Leader)
        {
            if (task.UserId != currentUser.Id) throw new ForbidException("Your not authorized");

        }

        var time = new TimeOnly().AddMinutes(request.Duration);

        task.IsComplete = true;
        task.Duration = time;
        await _todoRepository.SaveChanges();
    }
}
