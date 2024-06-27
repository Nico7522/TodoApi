
using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.UnassignLeader;

internal class UnassignLeaderByTeamCommandHandler : IRequestHandler<UnassignLeaderByTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;

    public UnassignLeaderByTeamCommandHandler(ITeamRepository teamRepository, UserManager<UserEntity> userManager)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
    }
    public async System.Threading.Tasks.Task Handle(UnassignLeaderByTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var userToDelete = team.Users.FirstOrDefault(u => u.Id == team.LeaderId);

        if (userToDelete is null) throw new ApiException("A error has happened, leader has not been removed");

        team.Users.Remove(userToDelete);
        team.Leader = null;

        await _teamRepository.SaveChanges();
    
    }
}
