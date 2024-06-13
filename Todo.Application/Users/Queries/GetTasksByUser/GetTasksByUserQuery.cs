using MediatR;
using Todo.Application.Task.Dto;
using Todo.Domain.Entities;

namespace Todo.Application.Users.Queries.GetTasksByUser;

public class GetTasksByUserQuery : IRequest<IEnumerable<TodoDto>>
{
    public string UserId { get; init; }
    public GetTasksByUserQuery(string userId)
    {
        UserId = userId;
    }
}
