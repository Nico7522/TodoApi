using MediatR;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.AddTask;

public class AssignTaskByTeamCommandHandler : IRequestHandler<AssignTaskByTeamCommand>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IAuthorization<TeamEntity> _authorization;


    public AssignTaskByTeamCommandHandler(ITodoRepository todoRepository, 
        ITeamRepository teamRepository, 
        IAuthorization<TeamEntity> authorization)
    {
        _todoRepository = todoRepository;
        _teamRepository = teamRepository;
        _authorization = authorization;
    }
    public async System.Threading.Tasks.Task Handle(AssignTaskByTeamCommand request, CancellationToken cancellationToken)
    {
        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        if (!_authorization.Authorize(team, Domain.Enums.RessourceOperation.Create)) throw new ForbidException("Your not authorized");

        if (task.TeamId == team.Id) throw new BadRequestException("Task already in team");

        if (task.TeamId != null || task.UserId != null) throw new BadRequestException("Task already assigned");

        team.Tasks.Add(task);
        await _todoRepository.SaveChanges();
    }
}
