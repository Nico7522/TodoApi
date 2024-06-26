using MediatR;

namespace Todo.Application.Team.Commands.UpdateTeamTask;

public class UpdateTeamTaskCommand : IRequest
{
    public Guid TeamId { get; init; }
    public Guid TaskId { get; init; }


    public UpdateTeamTaskCommand(Guid teamId, Guid taskId)
    {
        TeamId = teamId;
        TaskId = taskId;

    }
}
