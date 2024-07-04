using AutoMapper;
using MediatR;
using Todo.Application.Task.Dto;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Queries.GetAllActiveTasks;

public class GetAllActiveTasksQueryHandler : IRequestHandler<GetAllActiveTasksQuery, IEnumerable<TaskDto>>
{
    private readonly ITodoRepository _todoRepository;

    private readonly IMapper _mapper;
    public GetAllActiveTasksQueryHandler(ITodoRepository todoRepository, IMapper mapper)
    {
        _todoRepository = todoRepository;   
        _mapper = mapper;
    }
    public async System.Threading.Tasks.Task<IEnumerable<TaskDto>> Handle(GetAllActiveTasksQuery request, CancellationToken cancellationToken)
    {
        var activeTasks = await _todoRepository.GetAllActive();
        var TasksDto = _mapper.Map<IEnumerable<TaskDto>>(activeTasks);
        return TasksDto;
    }
}
