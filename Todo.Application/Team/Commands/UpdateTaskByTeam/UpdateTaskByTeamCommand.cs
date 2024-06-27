using MediatR;
using Todo.Domain.Enums;

namespace Todo.Application.Team.Commands.UpdateTeamTask;

public class UpdateTaskByTeamCommand : IRequest
{
    public Guid TaskId { get; init; }
    public Guid TeamId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public Priority Priority { get; init; }

    public UpdateTaskByTeamCommand(Guid taskId, Guid teamId, string title, string description, Priority priority)
    {
        TaskId = taskId;
        TeamId = teamId;
        Title = title;
        Description = description;
        Priority = priority;
    }
}
