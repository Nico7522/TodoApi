using AutoMapper;
using MediatR;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.UpdateTask;

internal class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly IMapper _mapper;
    private readonly ITodoRepository _todoRepository;
    public UpdateTaskCommandHandler(IMapper mapper, ITodoRepository todoRepository)
    {
        _mapper = mapper;
        _todoRepository = todoRepository;
    }
    public async System.Threading.Tasks.Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _todoRepository.GetById(request.Id);
        if (task is null) throw new ApiErrorException("Task not found", 404);
            
        _mapper.Map(request, task);
        await _todoRepository.SaveChanges();
    }
}
