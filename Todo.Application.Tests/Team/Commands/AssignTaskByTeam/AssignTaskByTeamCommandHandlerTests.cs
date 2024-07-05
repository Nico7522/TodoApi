using FluentAssertions;
using Moq;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;

namespace Todo.Application.Team.Commands.AddTask.Tests;

public class AssignTaskByTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<ITodoRepository> _taskRepositoryMock;
    private readonly Mock<IAuthorization<TeamEntity>> _teamAuthorizationMock;
    private readonly AssignTaskByTeamCommandHandler _handler;
    private readonly Guid _teamId;
    private readonly Guid _taskId;

    public AssignTaskByTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _taskRepositoryMock = new Mock<ITodoRepository>();
        _teamAuthorizationMock = new Mock<IAuthorization<TeamEntity>>();
        _teamId = Guid.NewGuid();
        _taskId = Guid.NewGuid();
        _handler = new AssignTaskByTeamCommandHandler(_taskRepositoryMock.Object, _teamRepositoryMock.Object, _teamAuthorizationMock.Object);
    }
    [Fact()]
    public async TaskAsync Handle_ForValidCommand_ShouldAssignTaskCorrectly()
    {
        // arrange
        var task = new TodoEntity()
        {
            Id = _taskId,
            Title = "Title",
            Description = "Description",
            Priority = Domain.Enums.Priority.High,
            Team = null,
            TeamId = null
        };

        var team = new TeamEntity() { Id = _teamId, Name = "team", Tasks = new List<TodoEntity>() };

        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _taskRepositoryMock.Setup(r => r.GetById(task.Id)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);

        var command = new AssignTaskByTeamCommand(_taskId, _teamId);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _taskRepositoryMock.Verify(r => r.SaveChanges(), Times.Once());
        _taskRepositoryMock.Verify(r => r.GetById(task.Id), Times.Once());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Once());

        team.Tasks.Should().HaveCount(1);
        team.Tasks.First().Id.Should().Be(task.Id);
    }

    [Fact()]
    public async TaskAsync Handle_NoExistingTeam_ShouldThrowNotFoundException()
    {
        // arrange
        var task = new TodoEntity()
        {
            Id = _taskId,
            Title = "Title",
            Description = "Description",
            Priority = Domain.Enums.Priority.High,
            Team = null,
            TeamId = null
        };

        var team = new TeamEntity() { Id = _teamId, Name = "team", Tasks = new List<TodoEntity>() };

        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync((TeamEntity?)null);
        _taskRepositoryMock.Setup(r => r.GetById(task.Id)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);

        var command = new AssignTaskByTeamCommand(_taskId, _teamId);

        // act

        Func<TaskAsync> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not found");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _taskRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        _taskRepositoryMock.Verify(r => r.GetById(task.Id), Times.Once());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Never());

        team.Tasks.Should().HaveCount(0);

    }

    [Fact()]
    public async TaskAsync Handle_NoExistingTask_ShouldThrowNotFoundException()
    {
        // arrange
        var task = new TodoEntity()
        {
            Id = _taskId,
            Title = "Title",
            Description = "Description",
            Priority = Domain.Enums.Priority.High,
            Team = null,
            TeamId = null
        };

        var team = new TeamEntity() { Id = _teamId, Name = "team", Tasks = new List<TodoEntity>() };

        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _taskRepositoryMock.Setup(r => r.GetById(task.Id)).ReturnsAsync((TodoEntity?)null);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);

        var command = new AssignTaskByTeamCommand(_taskId, _teamId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Task not found");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Never());
        _taskRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        _taskRepositoryMock.Verify(r => r.GetById(task.Id), Times.Once());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Never());

        team.Tasks.Should().HaveCount(0);

    }

    [Fact()]
    public async TaskAsync Handle_TaskAlreadyAssigned_ShouldThrowBadRequestException()
    {
        // arrange
        var team = new TeamEntity() { Id = _teamId, Name = "team", Tasks = new List<TodoEntity>() { new TodoEntity() { Id = _taskId } } };
        var task = new TodoEntity()
        {
            Id = _taskId,
            Title = "Title",
            Description = "Description",
            Priority = Domain.Enums.Priority.High,
            Team = team,
            TeamId = _teamId
        };


        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _taskRepositoryMock.Setup(r => r.GetById(task.Id)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);

        var command = new AssignTaskByTeamCommand(_taskId, _teamId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Task already in team");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _taskRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        _taskRepositoryMock.Verify(r => r.GetById(task.Id), Times.Once());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Once());

        team.Tasks.Should().HaveCount(1);
    }

    [Fact()]
    public async TaskAsync Handle_WithTaskAlreadyAssignedToAnotherTeam_ShouldThrowBadRequestException()
    {
        // arrange
        var team = new TeamEntity() { Id = _teamId, Name = "team", Tasks = new List<TodoEntity>() { } };
        var task = new TodoEntity()
        {
            Id = _taskId,
            Title = "Title",
            Description = "Description",
            Priority = Domain.Enums.Priority.High,
            Team = team,
            TeamId = Guid.NewGuid(),
        };


        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _taskRepositoryMock.Setup(r => r.GetById(task.Id)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);

        var command = new AssignTaskByTeamCommand(_taskId, _teamId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Task already assigned");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _taskRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        _taskRepositoryMock.Verify(r => r.GetById(task.Id), Times.Once());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Once());
        task.TeamId.Should().NotBe(team.Id);
    }

    [Fact()]
    public async TaskAsync Handle_WithTaskAlreadyAssignedToAnotherUser_ShouldThrowBadRequestException()
    {
        // arrange
        var team = new TeamEntity() { Id = _teamId, Name = "team", Tasks = new List<TodoEntity>() { } };
        var task = new TodoEntity()
        {
            Id = _taskId,
            Title = "Title",
            Description = "Description",
            Priority = Domain.Enums.Priority.High,
            Team = team,
            TeamId = null,
            UserId = "otherUserId"
        };


        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _taskRepositoryMock.Setup(r => r.GetById(task.Id)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);

        var command = new AssignTaskByTeamCommand(_taskId, _teamId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Task already assigned");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _taskRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        _taskRepositoryMock.Verify(r => r.GetById(task.Id), Times.Once());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Once());
        task.TeamId.Should().Be(null);
        task.UserId.Should().NotBe(null);
    }
}