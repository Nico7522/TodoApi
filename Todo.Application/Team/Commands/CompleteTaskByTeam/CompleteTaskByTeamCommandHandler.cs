
using MediatR;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Todo.Domain.Enums;
using Todo.Application.Users;
using Todo.Domain.Constants;

namespace Todo.Application.Team.Commands.CloseTask;

internal class CompleteTaskByTeamCommandHandler : IRequestHandler<CompleteTaskByTeamCommand>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IAuthorization<TeamEntity> _authorization;
    private IUserContext _userContext;
    public CompleteTaskByTeamCommandHandler(ITodoRepository todoRepository, ITeamRepository teamRepository, IAuthorization<TeamEntity> authorization, IUserContext userContext)
    {
        _todoRepository = todoRepository;
        _teamRepository = teamRepository;
        _authorization = authorization;
        _userContext = userContext;
    }
    public async System.Threading.Tasks.Task Handle(CompleteTaskByTeamCommand request, CancellationToken cancellationToken)
    {

        var currentUser = _userContext.GetCurrentUser();

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Task not found");

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        if (task.TeamId != team.Id) throw new BadRequestException("Task not in team");

        if(!_authorization.Authorize(team, RessourceOperation.Update, null)) throw new ForbidException("Your not authorized");

        //if (currentUser!.Role == UserRole.Leader || currentUser.Role == UserRole.User)
        //{
        //    var userId = team.Users.FirstOrDefault(u => u.Id == currentUser!.Id);
        //    if (userId is null) throw new ForbidException("Your not authorized");
        //}



        task.IsComplete = true;
        await _teamRepository.SaveChanges();
    }
}
