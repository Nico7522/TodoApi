using MediatR;
using Todo.Application.Team.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Team.Queries.GetAllTeams;

public class GetAllTeamQuery : IRequest<IEnumerable<TeamDto>>
{
    public bool IsActive { get; init; }

    public GetAllTeamQuery(bool isActive)
    {
        IsActive = isActive;
    }

}
