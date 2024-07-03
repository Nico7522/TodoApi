using AutoMapper;
using MediatR;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, string>
{
    IMapper _mapper;
    ITodoRepository _todoRepository;

    public CreateTaskCommandHandler(IMapper mapper, ITodoRepository todoRepository)
    {
        _mapper = mapper;
        _todoRepository = todoRepository;
    }
    public async Task<string> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        TodoEntity entity = _mapper.Map<TodoEntity>(request);
        var entityId = await _todoRepository.Create(entity);
        return entityId.ToString();
    }
}
