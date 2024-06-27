using MediatR;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.UnassignTask;

internal class UnassignTaskCommandHandler : IRequestHandler<UnassignTaskCommand>
{
    private readonly ITodoRepository _todoRepository;

    public UnassignTaskCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async System.Threading.Tasks.Task Handle(UnassignTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        task.User = null;
        task.Team = null;
        await _todoRepository.SaveChanges();
    }
}
