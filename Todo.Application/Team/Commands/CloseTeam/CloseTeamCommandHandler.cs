
using MediatR;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.CloseTeam;

internal class CloseTeamCommandHandler : IRequestHandler<CloseTeamCommand>
{
    private readonly ITeamRepository _teamRepository;
    public CloseTeamCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }
    public async System.Threading.Tasks.Task Handle(CloseTeamCommand request, CancellationToken cancellationToken)
    {
        var team = await _teamRepository.GetById(request.TeamId);
        if (team is null) throw new NotFoundException("Team not found");

        team.IsActive = false;
        await _teamRepository.SaveChanges();
    }
}
