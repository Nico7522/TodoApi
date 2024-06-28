using MediatR;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.UnassignLeader;

internal class UnassignLeaderFromTeamCommandHandler : IRequestHandler<UnassignLeaderFromTeamCommand>
{
    private readonly ITeamRepository _teamRepository;

    public UnassignLeaderFromTeamCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }
    public async System.Threading.Tasks.Task Handle(UnassignLeaderFromTeamCommand request, CancellationToken cancellationToken)
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
