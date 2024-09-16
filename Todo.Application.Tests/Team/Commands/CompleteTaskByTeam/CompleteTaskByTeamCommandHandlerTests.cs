using Azure.Core;
using FluentAssertions;
using Moq;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;

namespace Todo.Application.Team.Commands.CloseTask.Tests;

public class CompleteTaskByTeamCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepositoryMock;
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IAuthorization<TeamEntity>> _teamAuthorization;
    private readonly CompleteTaskByTeamCommandHandler _handler;
    private readonly Guid _taskId = Guid.NewGuid();
    private readonly Guid _teamId = Guid.NewGuid();
    private readonly TimeOnly _duration;

    public CompleteTaskByTeamCommandHandlerTests()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _teamAuthorization = new Mock<IAuthorization<TeamEntity>>();
        _handler = new CompleteTaskByTeamCommandHandler(_todoRepositoryMock.Object, _teamRepositoryMock.Object, _teamAuthorization.Object);
        _duration = new TimeOnly().AddMinutes(10);
    }
    [Fact()]
    public async void Handle_ForValidCommand_ShouldCompleteTaskCorrectly()
    {
        // arrange

        var task = new TodoEntity()
        {
            Id = _taskId,
            Title = "Title",
            Description = "Description",
            TeamId = _teamId,
            IsComplete = false,
        };

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            Tasks = new[] { task }
        };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _teamAuthorization.Setup(r => r.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new CompleteTaskByTeamCommand(_teamId, _taskId, 10);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Once());
        _teamAuthorization.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Once());
        task.IsComplete.Should().Be(true);
        task.Duration.Should().Be(_duration);
        task.ClosingDate.Should().Be(DateOnly.FromDateTime(DateTime.Now));
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
            TeamId = _teamId,
            IsComplete = false,
        };

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            Tasks = new[] { task }
        };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync((TeamEntity?)null);
        _teamAuthorization.Setup(r => r.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new CompleteTaskByTeamCommand(_teamId, _taskId, 10);

        // act

        Func<TaskAsync> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not found");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        _teamAuthorization.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Never());
        task.IsComplete.Should().Be(false);
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
            TeamId = _teamId,
            IsComplete = false,
        };

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            Tasks = new List<TodoEntity>()  
        };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _teamAuthorization.Setup(r => r.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new CompleteTaskByTeamCommand(_teamId, _taskId, 10);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Task not in team");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        _teamAuthorization.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Never());
        task.IsComplete.Should().Be(false);
    }

    [Fact()]
    public async TaskAsync Handle_TaskAlreadyComplete_ShouldThrowBadRequestException()
    {
        // arrange

        var task = new TodoEntity()
        {
            Id = _taskId,
            Title = "Title",
            Description = "Description",
            TeamId = _teamId,
            IsComplete = true,
        };

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            Tasks = new[] { task }
        };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _teamAuthorization.Setup(r => r.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new CompleteTaskByTeamCommand(_teamId, _taskId, 10);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Task is already completed");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        _teamAuthorization.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Never());
        task.IsComplete.Should().Be(true);
    }


    [Fact()]
    public async TaskAsync Handle_ForNotAuthorizedUser_ShouldThrowForbidException()
    {
        // arrange

        var task = new TodoEntity()
        {
            Id = _taskId,
            Title = "Title",
            Description = "Description",
            TeamId = _teamId,
            IsComplete = false,
        };

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            Tasks = new List<TodoEntity> { task }
        };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _teamAuthorization.Setup(r => r.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(false);
        var command = new CompleteTaskByTeamCommand(_teamId, _taskId, 10);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<ForbidException>().WithMessage("Your not authorized");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        _teamAuthorization.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Once());
        task.IsComplete.Should().Be(false);
    }

}