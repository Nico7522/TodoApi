using MediatR;
using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.AssignLeader;
public class AssignLeaderByTeamCommandHandler : IRequestHandler<AssignLeaderByTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    private readonly UserManager<UserEntity> _userManager;

    public AssignLeaderByTeamCommandHandler(
        ITeamRepository teamRepository, 
        UserManager<UserEntity> userManager
       )
    {
        _teamRepository = teamRepository;
        _userManager = userManager;
    }
    public async System.Threading.Tasks.Task Handle(AssignLeaderByTeamCommand request, CancellationToken cancellationToken)
    {

        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        if (team.LeaderId == user.Id) throw new BadRequestException("User is already leader of this team");

        if (user.TeamId != null) throw new BadRequestException("User is already in a team");

        if (team.LeaderId != null) throw new BadRequestException("Team has already a leader");

        team.LeaderId = user.Id;
        if(user.TeamId != team.Id) team.Users.Add(user);

        await _teamRepository.SaveChanges();
    }
}
