using Xunit;
using Moq;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using AsyncTask = System.Threading.Tasks.Task;
using FluentAssertions;
using Todo.Domain.Exceptions;

namespace Todo.Application.Team.Commands.UnassignTaskFromTeam.Tests;

public class UnassignTaskFromTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IAuthorization<TeamEntity>> _teamAuthorizationMock;
    private readonly Guid _teamId;
    private readonly Guid _taskId;
    private readonly UnassignTaskFromTeamCommandHandler _handler;

    public UnassignTaskFromTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _teamAuthorizationMock = new Mock<IAuthorization<TeamEntity>>();
        _teamId = Guid.NewGuid();
        _taskId = Guid.NewGuid();
        _handler = new UnassignTaskFromTeamCommandHandler(_teamRepositoryMock.Object, _teamAuthorizationMock.Object);
    }

    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldUnassignTaskFromTeamCorrectly()
    {
        // arrange

        var task = new TodoEntity { Id = _taskId };
        var team = new TeamEntity { Id = _teamId, Tasks = new List<TodoEntity> { new TodoEntity { Id = task.Id } } };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var command = new UnassignTaskFromTeamCommand(_teamId, _taskId);

        // act

        await _handler.Handle(command, CancellationToken.None);


        // assert

        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Once);
        team.Tasks.Should().BeEmpty();

    }

    [Fact()]
    public async AsyncTask Handle_ForTeamNotFound_ShouldThrowNotFoundException()
    {
        // arrange

        var task = new TodoEntity { Id = _taskId };
        var team = new TeamEntity { Id = _teamId, Tasks = new List<TodoEntity> { new TodoEntity { Id = task.Id } } };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync((TeamEntity?)null);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var command = new UnassignTaskFromTeamCommand(_teamId, _taskId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not found");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Never);
        team.Tasks.Should().NotBeEmpty();
    }

    [Fact()]
    public async AsyncTask Handle_ForTaskNotFound_ShouldThrowNotFoundException()
    {
        // arrange

        var task = new TodoEntity { Id = _taskId };
        var team = new TeamEntity { Id = _teamId, Tasks = new List<TodoEntity> { new TodoEntity { Id = Guid.NewGuid() } } };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var command = new UnassignTaskFromTeamCommand(_teamId, _taskId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Task not in team");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Never);
        team.Tasks.Should().NotBeEmpty();
    }

    [Fact()]
    public async AsyncTask Handle_ForUnauthorizedUserAction_ShouldThrowNotFoundException()
    {
        // arrange

        var task = new TodoEntity { Id = _taskId };
        var team = new TeamEntity { Id = _teamId, Tasks = new List<TodoEntity> { new TodoEntity { Id = _taskId } } };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(false);
        var command = new UnassignTaskFromTeamCommand(_teamId, _taskId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<ForbidException>().WithMessage("Your not authorized");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Once);
        team.Tasks.Should().NotBeEmpty();
    }
}