

using MediatR;
using Todo.Application.Team.Dto;

namespace Todo.Application.Team.Queries.GetTeamById;

public class GetTeamByIdQuery : IRequest<TeamDto?>
{
    public Guid TeamId { get; init; }
    public GetTeamByIdQuery(Guid teamId)
    {
        TeamId = teamId;
    }
}
