using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;

namespace Todo.Application.Users.Commands.DeleteUser.Tests;

public class DeleteUserCommandHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly string _userId;
    private readonly DeleteUserCommandHandler _handler;

    public DeleteUserCommandHandlerTests()
    {
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
        _userId = "userId";
        _handler = new DeleteUserCommandHandler(_userManagerMock.Object);
    }
    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldDeleteUserCorrectly()
    {
        // arrange
        var user = new UserEntity()
        {
            Id = _userId
        };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);
        var command = new DeleteUserCommand(_userId);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _userManagerMock.Verify(m => m.DeleteAsync(user), Times.Once);
    }

    [Fact()]
    public async AsyncTask Handle_ForUserNotFound_ShouldThrowNotFoundException()
    {
        // arrange
        var user = new UserEntity()
        {
            Id = _userId
        };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync((UserEntity?)null);
        _userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);
        var command = new DeleteUserCommand(_userId);

        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _userManagerMock.Verify(m => m.DeleteAsync(user), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForUsernotDeleted_ShouldThrowNotFoundException()
    {
        // arrange
        var user = new UserEntity()
        {
            Id = _userId
        };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Failed());
        var command = new DeleteUserCommand(_userId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<ApiException>().WithMessage("A error has occured");
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _userManagerMock.Verify(m => m.DeleteAsync(user), Times.Once);
    }
}