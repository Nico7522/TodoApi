﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Todo.Domain.Enums;
using Todo.Application.Users;
using Todo.Domain.Constants;

namespace Todo.Application.Team.Commands.DeleteUser;

internal class UnassignUserFromTeamCommandHandler : IRequestHandler<UnassignUserFromTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IAuthorization<TeamEntity> _teamAuthorizationService;
    private readonly IUserContext _userContext;

    public UnassignUserFromTeamCommandHandler(ITeamRepository teamRepository, 
        UserManager<UserEntity> userManager, 
        IAuthorization<TeamEntity> teamAuthorizationService,
        IUserContext userContext)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
        _teamAuthorizationService = teamAuthorizationService;
        _userContext = userContext;
    }
    public async System.Threading.Tasks.Task Handle(UnassignUserFromTeamCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        if (currentUser!.Role == UserRole.Leader)
        {
            if (team.LeaderId == user.Id) throw new ForbidException("Your not authorized");

        }
        if (user.TeamId != team.Id) throw new BadRequestException("User is not in team");



        team.Users.Remove(user);
        await _teamRepository.SaveChanges();

    }
}
