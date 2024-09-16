using MediatR;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.UnassignTaskFromTeam;

public class UnassignTaskFromTeamCommandHandler : IRequestHandler<UnassignTaskFromTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IAuthorization<TeamEntity> _teamAuthorizationService;

    public UnassignTaskFromTeamCommandHandler(ITeamRepository teamRepository,  IAuthorization<TeamEntity> teamAuthorizationService)
    {
        _teamRepository = teamRepository;
        _teamAuthorizationService = teamAuthorizationService;
    }

    public async System.Threading.Tasks.Task Handle(UnassignTaskFromTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var task = team.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task is null) throw new NotFoundException("Task not in team");

        if (!_teamAuthorizationService.Authorize(team, RessourceOperation.Delete)) throw new ForbidException("Your not authorized");

        team.Tasks.Remove(task);
        await _teamRepository.SaveChanges();

    }
}
