using FluentAssertions;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;
namespace Todo.Application.Team.Commands.CreateTeam.Tests;

public class CreateTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly CreateTeamCommandHandler _handler;

    public CreateTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _handler = new CreateTeamCommandHandler(_teamRepositoryMock.Object);
    }
    [Fact()]
    public async TaskAsync Handle_ForValidCommand_ShouldCreateTeamCorrectlyAndReturnTeamId()
    {
        // arrange
        var command = new CreateTeamCommand("test");
        var team = new TeamEntity() { Name = command.Name };
        _teamRepositoryMock.Setup(r => r.Create(team)).ReturnsAsync(team.Id);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _teamRepositoryMock.Verify(r => r.Create(It.Is<TeamEntity>(t => t.Id == team.Id && t.Name == team.Name)), Times.Once());
        
    }
}