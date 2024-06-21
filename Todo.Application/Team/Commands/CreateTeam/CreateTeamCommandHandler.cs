using MediatR;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;

namespace Todo.Application.Team.Commands.CreateTeam;

internal class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, Guid>
{
    private readonly ITeamRepository _teamRepository;
    public CreateTeamCommandHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }
    public async Task<Guid> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        TeamEntity newTeam = new TeamEntity() { Name = request.Name};
        var id = await _teamRepository.Create(newTeam);
        return id;
    }
}
