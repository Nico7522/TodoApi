
using AutoMapper;
using MediatR;
using Todo.Application.Users;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.UpdateTeamTask;

internal class UpdateTaskByTeamCommandHandler : IRequestHandler<UpdateTaskByTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITodoRepository _todoRepository;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IAuthorization<TeamEntity> _authorization;

    public UpdateTaskByTeamCommandHandler(ITeamRepository teamRepository, 
        ITodoRepository todoRepository, 
        IUserContext userContext, 
        IMapper mapper,
        IAuthorization<TeamEntity> authorization)
    {
        _teamRepository = teamRepository;
        _todoRepository = todoRepository;
        _userContext = userContext;
        _mapper = mapper;
        _authorization = authorization;
    }
    public async System.Threading.Tasks.Task Handle(UpdateTaskByTeamCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        if (task.TeamId != team.Id) throw new BadRequestException("Task not in team");

        if (!_authorization.Authorize(team, RessourceOperation.Update, null)) throw new ForbidException("Your not authorized");

        //if (currentUser!.Role == UserRole.Leader)
        //{
        //    if (team.LeaderId != currentUser.Id) throw new ForbidException("Your not authorized");
        //}


        // TODO: faire les modifs

        _mapper.Map(request, task);
        await _todoRepository.SaveChanges();


    }
}
