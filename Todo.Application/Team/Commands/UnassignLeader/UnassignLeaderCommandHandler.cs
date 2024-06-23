
using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.UnassignLeader;

internal class UnassignLeaderCommandHandler : IRequestHandler<UnassignLeaderCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;

    public UnassignLeaderCommandHandler(ITeamRepository teamRepository, UserManager<UserEntity> userManager)
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
    }
    public async System.Threading.Tasks.Task Handle(UnassignLeaderCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");


        var user = team.Users.FirstOrDefault(u => u.Id == team.LeaderId);
        if (user is not null)
        {
            team.Users.Remove(user);
        }

        team.LeaderId = null;
        await _teamRepository.SaveChanges();
    
    }
}
