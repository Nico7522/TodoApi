using MediatR;

namespace Todo.Application.Users.Commands.AssignTaskByUser;

public class AssignTaskByUserCommand : IRequest<bool>
{
    public string UserId { get; init; }
    public Guid TaskId { get; init; }

    public AssignTaskByUserCommand(string userId, Guid taskId)
    {
        UserId = userId;
        TaskId = taskId;
    }
}
