using AutoMapper;
using MediatR;
using Todo.Application.Task.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Queries.GetAllActiveTasks;

internal class GetAllActiveTasksQueryHandler : IRequestHandler<GetAllActiveTasksQuery, IEnumerable<TodoDto>>
{
    private readonly ITodoRepository _todoRepository;

    private readonly IMapper _mapper;
    public GetAllActiveTasksQueryHandler(ITodoRepository todoRepository, IMapper mapper)
    {
        _todoRepository = todoRepository;   
        _mapper = mapper;
    }
    public async System.Threading.Tasks.Task<IEnumerable<TodoDto>> Handle(GetAllActiveTasksQuery request, CancellationToken cancellationToken)
    {
        var activeTasks = await _todoRepository.GetAllActive();
        var TasksDto = _mapper.Map<IEnumerable<TodoDto>>(activeTasks);
        return TasksDto;
    }
}
