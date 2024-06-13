

using AutoMapper;
using MediatR;
using Todo.Application.Task.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Queries.GetTaskById;

internal class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TodoDto>
{
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;
    public GetTaskByIdQueryHandler(ITodoRepository todoRepository, IMapper mapper)
    {
        _todoRepository = todoRepository;
        _mapper = mapper;
    }
    public async Task<TodoDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new ApiErrorException("Task not found", 404);
        var taskDto = _mapper.Map<TodoDto>(task);
        return taskDto;
    }
}
