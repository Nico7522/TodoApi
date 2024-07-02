
using MediatR;

namespace Todo.Application.Users.Queries.GetTasksNumberByUser;

public class GetTasksNumberByUserQuery : IRequest<int>
{
    public string UserId { get; init; }
    public GetTasksNumberByUserQuery(string userId)
    {
        UserId = userId;  
    }
}
