using MediatR;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.CompleteTask;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IAuthorization<TodoEntity> _authorization;
    public CompleteTaskCommandHandler(ITodoRepository todoRepository, IAuthorization<TodoEntity> authorization)
    {
        _todoRepository = todoRepository;
        _authorization = authorization;
    }
    public async System.Threading.Tasks.Task Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        if (!_authorization.Authorize(task, Domain.Enums.RessourceOperation.Update)) throw new ForbidException("Your not authorized");
        //if (currentUser!.Role == UserRole.User || currentUser.Role == UserRole.Leader)
        //{
        //    if (task.UserId != currentUser.Id) throw new ForbidException("Your not authorized");

        //}

        var time = new TimeOnly().AddMinutes(request.Duration);

        task.IsComplete = true;
        task.Duration = time;
        task.ClosingDate = DateOnly.FromDateTime(DateTime.Now);
        await _todoRepository.SaveChanges();
    }
}
