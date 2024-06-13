using MediatR;

namespace Todo.Application.Users.Commands.AssignTaskByUser;

public class AssignTaskByUserCommand : IRequest<bool>
{
    public string UserId { get; init; }
    public string TaskId { get; init; }

    public AssignTaskByUserCommand(string userId, string taskId)
    {
        UserId = userId;
        TaskId = taskId;
    }
}
