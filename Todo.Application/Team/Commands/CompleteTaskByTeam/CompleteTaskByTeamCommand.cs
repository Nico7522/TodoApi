using MediatR;

namespace Todo.Application.Team.Commands.CloseTask;

public class CompleteTaskByTeamCommand : IRequest
{
    public Guid TeamId { get; init; }

    public Guid TaskId { get; init; }
    public int Duration { get; set; }

    public CompleteTaskByTeamCommand(Guid teamId, Guid taskId, int duration)
    {
        TeamId = teamId;
        TaskId = taskId;
        Duration = duration;
    }
}
