﻿using MediatR;
using Todo.Application.Users;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.AddTask;

internal class AssignTaskByTeamCommandHandler : IRequestHandler<AssignTaskByTeamCommand>
{
    private readonly ITodoRepository _todoRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IAuthorization<TeamEntity> _authorization;
    private readonly IUserContext _userContext;


    public AssignTaskByTeamCommandHandler(ITodoRepository todoRepository, ITeamRepository teamRepository, IAuthorization<TeamEntity> authorization, IUserContext userContext)
    {
        _todoRepository = todoRepository;
        _teamRepository = teamRepository;
        _authorization = authorization;
        _userContext = userContext;
    }
    public async System.Threading.Tasks.Task Handle(AssignTaskByTeamCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var task = await _todoRepository.GetById(request.TaskId);
        if (task is null) throw new NotFoundException("Task not found");

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        if (task.TeamId == team.Id) throw new BadRequestException("Task already in team");

        if (task.TeamId != null || task.UserId != null) throw new BadRequestException("Task already assigned");

        if (currentUser!.Role == UserRole.Leader && currentUser.Id != team.LeaderId) throw new ForbidException("Your not authorized");

        team.Tasks.Add(task);
        await _todoRepository.SaveChanges();


    }
}
