using MediatR;
using Todo.Application.Team.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Users.Queries.GetTeamByUser;

public class GetTeamByUserQuery : IRequest<TeamDto>
{
    public string UserId { get; init; }

    public GetTeamByUserQuery(string userId)
    {
        UserId = userId;
    }
}
