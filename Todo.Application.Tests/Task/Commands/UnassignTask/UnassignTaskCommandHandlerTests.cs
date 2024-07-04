using FluentAssertions;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;

namespace Todo.Application.Task.Commands.UnassignTask.Tests;

public class UnassignTaskCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepositoryMock;
    private readonly UnassignTaskCommandHandler _handler;
    public UnassignTaskCommandHandlerTests()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _handler = new UnassignTaskCommandHandler(_todoRepositoryMock.Object);
    }

    [Fact()]
    public async System.Threading.Tasks.Task Handle_WithValidRequest_ShouldUnassignTaskCorrectly()
    {
        // arrange
        var taskId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var team = new TeamEntity()
        {
            Id = teamId,
            Name = "team"
        };
        var userId = "idUser";
        var user = new UserEntity()
        {
            Id = userId,
            Email = "test@gmail.com"
        };
        var taskEntity = new TodoEntity()
        {
            Id = taskId,
            Title = "Title",
            Description = "Description", 
            Priority = Domain.Enums.Priority.High,
            TeamId = teamId,
            Team = team,
            UserId = userId,    
            User = user,
        };

        var command = new UnassignTaskCommand(taskEntity.Id);
        _todoRepositoryMock.Setup(r => r.GetById(taskEntity.Id)).ReturnsAsync(taskEntity);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        taskEntity.User.Should().Be(null);
        taskEntity.Team.Should().Be(null);

    }

    [Fact()]
    public async System.Threading.Tasks.Task Handle_WithNoExistingTask_ShouldThrowNotFoundException()
    {
        // arrange
        var taskId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var team = new TeamEntity()
        {
            Id = teamId,
            Name = "team"
        };
        var userId = "idUser";
        var user = new UserEntity()
        {
            Id = userId,
            Email = "test@gmail.com"
        };
        var taskEntity = new TodoEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Title",
            Description = "Description",
            Priority = Domain.Enums.Priority.High,
            TeamId = teamId,
            Team = team,
            UserId = userId,
            User = user,
        };

        var command = new UnassignTaskCommand(taskEntity.Id);
        _todoRepositoryMock.Setup(r => r.GetById(taskEntity.Id)).ReturnsAsync((TodoEntity?)null);

        // act

        Func<AsyncTask> act =  async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Task not found");
        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        taskEntity.User.Should().Be(user);
        taskEntity.Team.Should().Be(team);

    }
}