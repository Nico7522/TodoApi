using AutoMapper;
using FluentValidation;
using MediatR;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Task.Commands.UpdateTask;

internal class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand>
{
    private readonly IMapper _mapper;
    private readonly ITodoRepository _todoRepository;
    private readonly IValidator<UpdateTaskCommand> _validator;
    public UpdateTaskCommandHandler(IMapper mapper, ITodoRepository todoRepository, IValidator<UpdateTaskCommand> validator)
    {
        _mapper = mapper;
        _todoRepository = todoRepository;
        _validator = validator;
    }
    public async System.Threading.Tasks.Task Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _todoRepository.GetById(request.Id);
        if (task is null) throw new NotFoundException("Task not found");


        var result = await _validator.ValidateAsync(request);
        if (result.Errors.Any())
        {
            throw new ValidationException(result.Errors);
        }

        _mapper.Map(request, task);
        await _todoRepository.SaveChanges();
    }
}
