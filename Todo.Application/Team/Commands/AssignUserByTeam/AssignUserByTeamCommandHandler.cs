﻿using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Application.Users;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.AddUser;

internal class AssignUserByTeamCommandHandler : IRequestHandler<AssignUserByTeamCommand>
{

    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IAuthorization<TeamEntity> _teamAuthorizationService;
    private readonly IUserContext _userContext;

    public AssignUserByTeamCommandHandler(ITeamRepository teamRepository, 
        UserManager<UserEntity> userManager, 
        IAuthorization<TeamEntity> teamAuthorizationService,
        IUserContext userContext)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
        _teamAuthorizationService = teamAuthorizationService;
        _userContext = userContext;
    }
    public async System.Threading.Tasks.Task Handle(AssignUserByTeamCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var user = await _userManager.Users.Include(u => u.Team).FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        if (user.Team != null && user.Team.Id == team.Id) throw new BadRequestException("User already in team");

        if (user.Team != null)
        {
            if (user.Team.Id != team.Id && user.Team.IsActive) throw new BadRequestException("User already in another team");
        }

        if (team.LeaderId != currentUser!.Id) throw new ForbidException("Your not authorized");


            
        team.Users.Add(user);
        await _teamRepository.SaveChanges();
    }
}
