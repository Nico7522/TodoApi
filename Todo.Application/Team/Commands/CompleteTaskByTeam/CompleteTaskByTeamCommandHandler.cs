using MediatR;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Todo.Domain.Enums;

namespace Todo.Application.Team.Commands.CloseTask;

public class CompleteTaskByTeamCommandHandler : IRequestHandler<CompleteTaskByTeamCommand>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IAuthorization<TeamEntity> _authorization;
    public CompleteTaskByTeamCommandHandler(ITodoRepository todoRepository, 
        ITeamRepository teamRepository, 
        IAuthorization<TeamEntity> authorization)
    {
        _todoRepository = todoRepository;
        _teamRepository = teamRepository;
        _authorization = authorization;
    }
    public async System.Threading.Tasks.Task Handle(CompleteTaskByTeamCommand request, CancellationToken cancellationToken)
    {

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var task = team.Tasks.FirstOrDefault(t => t.Id == request.TaskId);
        if (task is null) throw new NotFoundException("Task not in team");

        if (task.IsComplete) throw new BadRequestException("Task is already completed");

        if(!_authorization.Authorize(team, RessourceOperation.Update)) throw new ForbidException("Your not authorized");

        task.IsComplete = true;
        task.Duration = new TimeOnly().AddMinutes(request.Duration);
        task.ClosingDate = DateOnly.FromDateTime(DateTime.Now);
        await _teamRepository.SaveChanges();
    }
}
