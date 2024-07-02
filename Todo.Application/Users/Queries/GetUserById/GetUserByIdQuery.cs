
using MediatR;
using Todo.Application.Users.Dto;

namespace Todo.Application.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public string UserId { get; init; }
    public GetUserByIdQuery(string userId)
    {
        UserId = userId;
    }
}
