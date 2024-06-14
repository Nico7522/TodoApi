using MediatR;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.CompleteTask;

public class CompleteTaskCommandHandler : IRequestHandler<CompleteTaskCommand, bool>
{
    private readonly ITodoRepository _todoRepository;
    public CompleteTaskCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }
    public async Task<bool> Handle(CompleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new ApiErrorException("Task not found", 404);

        var time = new TimeOnly().AddMinutes(request.Duration);

        task.IsComplete = true;
        task.Duration = time;
        await _todoRepository.SaveChanges();

        return true;
    }
}
