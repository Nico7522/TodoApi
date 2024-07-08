using FluentAssertions;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;

namespace Todo.Application.Team.Commands.CloseTeam.Tests;

public class CloseTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly CloseTeamCommandHandler _handler;
    private readonly Guid _teamId;

    public CloseTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _handler = new CloseTeamCommandHandler(_teamRepositoryMock.Object);
        _teamId = Guid.NewGuid();
    }
    [Fact()]
    public async void Handle_ForValidCommand_ShouldCloseTeamCorrectly()
    {
        // arrange

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            IsActive = true,
        };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        var command = new CloseTeamCommand(_teamId);
        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Once());
        team.IsActive.Should().Be(false);

    }

    [Fact()]
    public async TaskAsync Handle_NoExistingTeam_ShouldThrowNotFoundException()
    {
        // arrange

        var team = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            IsActive = true,
        };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync((TeamEntity?)null);
        var command = new CloseTeamCommand(_teamId);
        // act

        Func<TaskAsync> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not found");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        team.IsActive.Should().Be(true);

    }

    [Fact()]
    public async TaskAsync Handle_ForTeamAlreadyInactive_ShouldThrowBadRequestException()
    {
        // arrange

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            IsActive = false,
        };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        var command = new CloseTeamCommand(_teamId);
        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Team is already inactive");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        team.IsActive.Should().Be(false);

    }


}