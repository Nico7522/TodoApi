
using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.DeleteUser;

internal class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly IAuthorization<TeamEntity> _teamAuthorizationService;

    public DeleteUserCommandHandler(ITeamRepository teamRepository, UserManager<UserEntity> userManager, IAuthorization<TeamEntity> teamAuthorizationService)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
        _teamAuthorizationService = teamAuthorizationService;
    }
    public async System.Threading.Tasks.Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");


        if (!_teamAuthorizationService.Authorize(team, Domain.Enums.RessourceOperation.Delete, user.Id)) throw new ForbidException("Your not authorized");
    


        if (user.TeamId != team.Id) throw new BadRequestException("User is not in team");

        team.Users.Remove(user);
        await _teamRepository.SaveChanges();

    }
}
