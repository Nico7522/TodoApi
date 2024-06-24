using MediatR;

namespace Todo.Application.Users.Commands.UnassignTaskByUser;

public class UnassignTaskByUserCommand : IRequest
{
    public string UserId { get; init; }
    public Guid TaskId { get; init; }
    public UnassignTaskByUserCommand(string userId, Guid taskId)
    {
        UserId = userId;
        TaskId = taskId;
    }
}
