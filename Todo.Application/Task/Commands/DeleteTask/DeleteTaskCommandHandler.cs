using MediatR;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.DeleteTask;

internal class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand>
{
    private readonly ITodoRepository _todoRepository;

    public DeleteTaskCommandHandler(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async System.Threading.Tasks.Task Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new ApiErrorException("Task not found", 404);
        await _todoRepository.Delete(task);
    }
}
