using MediatR;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.CompleteTask;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
{
    private readonly ITodoRepository _todoRepository;

    private readonly IAuthorization<TodoEntity> _authorization;
    public CompleteTaskCommandHandler(ITodoRepository todoRepository, IAuthorization<TodoEntity> authorization)
    {
        _todoRepository = todoRepository;
        _authorization = authorization;
    }
    public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        if (!_authorization.Authorize(task, Domain.Enums.RessourceOperation.Update, null)) throw new ForbidException("Your not authorized");

        var time = new TimeOnly().AddMinutes(request.Duration);

        task.IsComplete = true;
        task.Duration = time;
        await _todoRepository.SaveChanges();

        return true;
    }
}
