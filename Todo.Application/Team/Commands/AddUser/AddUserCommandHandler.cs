using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.AddUser;

internal class AddUserCommandHandler : IRequestHandler<AddUserCommand>
{

    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly ITeamAuthorizationService _teamAuthorizationService;

    public AddUserCommandHandler(ITeamRepository teamRepository, UserManager<UserEntity> userManager, ITeamAuthorizationService teamAuthorizationService)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
        _teamAuthorizationService = teamAuthorizationService;
    }
    public async System.Threading.Tasks.Task Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var user = await _userManager.Users.Include(u => u.Team).FirstOrDefaultAsync(u => u.Id == request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        if (!_teamAuthorizationService.Authorize(team, Domain.Enums.RessourceOperation.Create)) throw new ForbidException("Your not authorized");

        if (user.Team != null && user.Team.Id == team.Id) throw new BadRequestException("User already in team");

        if (user.Team != null)
        {
            if (user.Team.Id != team.Id && user.Team.IsActive) throw new BadRequestException("User already in another team");
        }
            
        team.Users.Add(user);
        await _teamRepository.SaveChanges();
    }
}
