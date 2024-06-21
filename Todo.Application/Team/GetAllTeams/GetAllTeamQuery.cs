using MediatR;
using Todo.Application.Team.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Team.GetAllTeams;

public class GetAllTeamQuery : IRequest<IEnumerable<TeamDto>>
{
    public bool IsActive { get; set; }
 
}
