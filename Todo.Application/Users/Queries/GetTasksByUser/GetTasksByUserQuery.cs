using MediatR;
using Todo.Domain.Entities;

namespace Todo.Application.Users.Queries.GetTasksByUser;

public class GetTasksByUserQuery : IRequest<IEnumerable<TodoEntity>>
{
    public string UserId { get; init; }
    public GetTasksByUserQuery(string userId)
    {
        UserId = userId;
    }
}
