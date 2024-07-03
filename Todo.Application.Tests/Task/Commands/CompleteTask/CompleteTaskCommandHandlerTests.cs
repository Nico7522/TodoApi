using Xunit;
using Moq;
using Todo.Domain.Repositories;
using Todo.Domain.Entities;
using Todo.Application.Users;
using FluentAssertions;
using Todo.Domain.Exceptions;
using Todo.Domain.AuthorizationInterfaces;

namespace Todo.Application.Task.Commands.CompleteTask.Tests;

public class CompleteTaskCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepositoryMock;
    private readonly CompleteTaskCommandHandler _handler;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IAuthorization<TodoEntity>> _authorizationMock;
    public CompleteTaskCommandHandlerTests()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _userContextMock = new Mock<IUserContext>();
        _authorizationMock = new Mock<IAuthorization<TodoEntity>>();
        _handler = new CompleteTaskCommandHandler(_todoRepositoryMock.Object, _authorizationMock.Object);
    }

    [Fact()]
    public async System.Threading.Tasks.Task Handle_WithValidRequest_ShouldCompleteTaskCorrectly()
    {
        // arrange
        var guid = new Guid();
        var taskToComplete = new TodoEntity()
        {
            Id = guid,
            Title = "test",
            Description = "description",
            IsComplete = false,
            CreationDate = new DateOnly(2020, 01, 01),
            Priority = Domain.Enums.Priority.Low,
            UserId = "id"
        };

        _todoRepositoryMock.Setup(repo => repo.GetById(guid)).ReturnsAsync(taskToComplete);
        _authorizationMock.Setup(a => a.Authorize(taskToComplete, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new CompleteTaskCommand(guid, 30);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        taskToComplete.IsComplete.Should().Be(true);
        taskToComplete.Duration.Should().NotBeNull();
    }


    [Fact()]
    public async System.Threading.Tasks.Task Handle_NoExistingTask_ShouldThrowNotFoundException()
    {
        // arrange
        var guid = new Guid();
        var taskToComplete = new TodoEntity()
        {
            Id = guid,
            Title = "test",
            Description = "description",
            IsComplete = false,
            CreationDate = new DateOnly(2020, 01, 01),
            Priority = Domain.Enums.Priority.Low,
            UserId = "id"
        };

        _todoRepositoryMock.Setup(repo => repo.GetById(guid)).ReturnsAsync((TodoEntity?)null);
        _authorizationMock.Setup(a => a.Authorize(taskToComplete, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new CompleteTaskCommand(guid, 30);

        // act

        Func<System.Threading.Tasks.Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Task not found");
    }

    [Fact()]
    public async System.Threading.Tasks.Task Handle_ForValidRequestWithUnauthorizedUser_ShouldThrowForbidException()
    {
        // arrange
        var guid = new Guid();
        var taskToComplete = new TodoEntity()
        {
            Id = guid,
            Title = "test",
            Description = "description",
            IsComplete = false,
            CreationDate = new DateOnly(2020, 01, 01),
            Priority = Domain.Enums.Priority.Low,
            UserId = "id"
        };

        _todoRepositoryMock.Setup(repo => repo.GetById(guid)).ReturnsAsync(taskToComplete);
        _authorizationMock.Setup(a => a.Authorize(taskToComplete, Domain.Enums.RessourceOperation.Update)).Returns(false);
        var command = new CompleteTaskCommand(guid, 30);

        // act

        Func<System.Threading.Tasks.Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<ForbidException>().WithMessage("Your not authorized");
    }

}
