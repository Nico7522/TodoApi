using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.UnassignLeader;

public class UnassignLeaderFromTeamCommandHandler : IRequestHandler<UnassignLeaderFromTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;

    public UnassignLeaderFromTeamCommandHandler(ITeamRepository teamRepository, UserManager<UserEntity> userManager)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
    }
    public async System.Threading.Tasks.Task Handle(UnassignLeaderFromTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var user = await _userManager.FindByIdAsync(team.LeaderId!);
        if (user is null) throw new NotFoundException("Team leader not found");

        team.Users.Remove(user);
        team.Leader = null;

        await _teamRepository.SaveChanges();
    
    }
}
