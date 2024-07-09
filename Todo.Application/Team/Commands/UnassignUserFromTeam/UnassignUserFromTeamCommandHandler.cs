using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Todo.Domain.Enums;

namespace Todo.Application.Team.Commands.DeleteUser;

public class UnassignUserFromTeamCommandHandler : IRequestHandler<UnassignUserFromTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IAuthorization<TeamEntity> _teamAuthorizationService;

    public UnassignUserFromTeamCommandHandler(ITeamRepository teamRepository, 
        UserManager<UserEntity> userManager, 
        IAuthorization<TeamEntity> teamAuthorizationService)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
        _teamAuthorizationService = teamAuthorizationService;
    }
    public async System.Threading.Tasks.Task Handle(UnassignUserFromTeamCommand request, CancellationToken cancellationToken)
    {

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        if (!_teamAuthorizationService.Authorize(team, RessourceOperation.Delete)) throw new ForbidException("Your not authorized");

        if (user.Id == team.LeaderId) throw new BadRequestException("You cannot remove yourself from your team");

        if (user.TeamId != team.Id) throw new BadRequestException("User is not in team");



        team.Users.Remove(user);
        await _teamRepository.SaveChanges();

    }
}
