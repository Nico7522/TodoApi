using MediatR;

namespace Todo.Application.Team.Commands.CreateTeam;
public class CreateTeamCommand : IRequest<Guid>
{
    public string Name { get; init; }

    public CreateTeamCommand(string name)
    {
        Name = name;
    }
}
