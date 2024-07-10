using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;


namespace Todo.Application.Users.Commands.AssignTaskByUser.Tests;

public class AssignTaskByUserCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepositoryMock;
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly AssignTaskByUserCommandHandler _handler;
    private readonly Guid _taskId;
    private readonly string _userId;


    public AssignTaskByUserCommandHandlerTests()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _userManagerMock = new Mock<UserManager<UserEntity>>(new Mock<IUserStore<UserEntity>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<UserEntity>>().Object,
                new IUserValidator<UserEntity>[0],
                new IPasswordValidator<UserEntity>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<UserEntity>>>().Object
                );
        _handler = new AssignTaskByUserCommandHandler(_userManagerMock.Object, _todoRepositoryMock.Object);
        _taskId = Guid.NewGuid();
        _userId = "userId";
    }

    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldAssignTaskToUserCorrectly()
    {

        // arrange

        var task = new TodoEntity() { Id = _taskId };
        var user = new UserEntity() { Id = _userId, Tasks = new List<TodoEntity>() { } };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        var command = new AssignTaskByUserCommand(_userId, _taskId);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _todoRepositoryMock.Verify(m => m.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(m => m.SaveChanges(), Times.Once);

        user.Tasks.Should().NotBeEmpty();
        user.Tasks.Should().HaveCount(1);
    }

    [Fact()]
    public async AsyncTask Handle_ForUserNotFound_ShouldThrowNotFoundException()
    {

        // arrange

        var task = new TodoEntity() { Id = _taskId };
        var user = new UserEntity() { Id = _userId, Tasks = new List<TodoEntity>() { } };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync((UserEntity?)null);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        var command = new AssignTaskByUserCommand(_userId, _taskId);

        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _todoRepositoryMock.Verify(m => m.GetById(_taskId), Times.Never);
        _todoRepositoryMock.Verify(m => m.SaveChanges(), Times.Never);

        user.Tasks.Should().BeEmpty();
    }

    [Fact()]
    public async AsyncTask Handle_ForTaskNotFound_ShouldThrowNotFoundException()
    {

        // arrange

        var task = new TodoEntity() { Id = _taskId };
        var user = new UserEntity() { Id = _userId, Tasks = new List<TodoEntity>() { } };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync((TodoEntity?)null);
        var command = new AssignTaskByUserCommand(_userId, _taskId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Task not found");
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _todoRepositoryMock.Verify(m => m.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(m => m.SaveChanges(), Times.Never);

        user.Tasks.Should().BeEmpty();
    }

    [Fact()]
    public async AsyncTask Handle_TaskAlreadyAssignedToTheUser_ShouldBadRequestExceptionCorrectly()
    {

        // arrange

        var task = new TodoEntity() { Id = _taskId, UserId = _userId };
        var user = new UserEntity() { Id = _userId, Tasks = new List<TodoEntity>() { task } };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        var command = new AssignTaskByUserCommand(_userId, _taskId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Task already assigned to this user");

        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _todoRepositoryMock.Verify(m => m.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(m => m.SaveChanges(), Times.Never);

        user.Tasks.Should().NotBeEmpty();
        user.Tasks.Should().HaveCount(1);
    }

    [Fact()]
    public async AsyncTask Handle_TaskAlreadyAssignedToAnotherUser_ShouldBadRequestExceptionCorrectly()
    {

        // arrange

        var task = new TodoEntity() { Id = _taskId, UserId = "id" };
        var user = new UserEntity() { Id = _userId, Tasks = new List<TodoEntity>() { } };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        var command = new AssignTaskByUserCommand(_userId, _taskId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Task is already assigned");

        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _todoRepositoryMock.Verify(m => m.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(m => m.SaveChanges(), Times.Never);

    }

    [Fact()]
    public async AsyncTask Handle_TaskAlreadyAssignedToTeam_ShouldBadRequestExceptionCorrectly()
    {

        // arrange

        var task = new TodoEntity() { Id = _taskId, TeamId = Guid.NewGuid() };
        var user = new UserEntity() { Id = _userId, Tasks = new List<TodoEntity>() { } };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        var command = new AssignTaskByUserCommand(_userId, _taskId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Task is already assigned");

        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _todoRepositoryMock.Verify(m => m.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(m => m.SaveChanges(), Times.Never);

    }
}