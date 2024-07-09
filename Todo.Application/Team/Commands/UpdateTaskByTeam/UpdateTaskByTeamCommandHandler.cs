using AutoMapper;
using FluentValidation;
using MediatR;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.UpdateTeamTask;

public class UpdateTaskByTeamCommandHandler : IRequestHandler<UpdateTaskByTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITodoRepository _todoRepository;
    private readonly IMapper _mapper;
    private readonly IAuthorization<TeamEntity> _authorization;
    private readonly IValidator<UpdateTaskByTeamCommand> _validator;

    public UpdateTaskByTeamCommandHandler(ITeamRepository teamRepository, 
        ITodoRepository todoRepository, 
        IMapper mapper,
        IAuthorization<TeamEntity> authorization,
        IValidator<UpdateTaskByTeamCommand> validator)
    {
        _teamRepository = teamRepository;
        _todoRepository = todoRepository;
        _mapper = mapper;
        _authorization = authorization;
        _validator = validator;
    }
    public async System.Threading.Tasks.Task Handle(UpdateTaskByTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        if (task.TeamId != team.Id) throw new BadRequestException("Task not in team");

        if (!_authorization.Authorize(team, RessourceOperation.Update)) throw new ForbidException("Your not authorized");

        var result = await _validator.ValidateAsync(request);
        if (result.Errors.Any())
        {
            throw new ValidationException(result.Errors);
        }

        _mapper.Map(request, task);
        await _todoRepository.SaveChanges();
    }
}
