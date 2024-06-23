
using MediatR;

namespace Todo.Application.Team.Commands.CloseTeam;

public class CloseTeamCommand : IRequest
{
    public Guid TeamId { get; init; }
    public CloseTeamCommand(Guid teamId)
    {
        TeamId = teamId;
    }
}
