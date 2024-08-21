using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Web;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Xunit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
namespace Todo.Application.Users.Commands.ConfirmEmail.Tests;

public class ConfirmEmailCommandHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly ConfirmEmailCommandHandler _handler;
    private readonly string _userId;
    public ConfirmEmailCommandHandlerTests()
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
        _handler = new ConfirmEmailCommandHandler(_userManagerMock.Object);
        _userId = "userId";
    }
    [Fact()]
    public async void Handle_ForValidCommand_ShouldConfirmAccount()
    {
        // arrange

        var token = HttpUtility.UrlDecode("token");
        var user = new UserEntity() { Id = _userId };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);
        var command = new ConfirmEmailCommand(_userId, token);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _userManagerMock.Verify(m => m.ConfirmEmailAsync(user, token), Times.Once);

    }


    [Fact()]
    public async void Handle_InvalidCommandUserIdNull_ShouldThrowBadRequestException()
    {
        // arrange

        var token = HttpUtility.UrlDecode("token");
        var user = new UserEntity() { Id = _userId };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);
        var command = new ConfirmEmailCommand(null, token);

        // act

        Func<System.Threading.Tasks.Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Link expired");
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Never);
        _userManagerMock.Verify(m => m.ConfirmEmailAsync(user, token), Times.Never);

    }

    [Fact()]
    public async void Handle_InvalidCommandTokenNull_ShouldThrowBadRequestException()
    {
        // arrange

        var token = HttpUtility.UrlDecode("token");
        var user = new UserEntity() { Id = _userId };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);
        var command = new ConfirmEmailCommand(_userId, null);

        // act

        Func<System.Threading.Tasks.Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Link expired");
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Never);
        _userManagerMock.Verify(m => m.ConfirmEmailAsync(user, token), Times.Never);

    }

    [Fact()]
    public async void Handle_UserNotFound_ShouldThrowNotFoundException()
    {
        // arrange

        var token = HttpUtility.UrlDecode("token");
        var user = new UserEntity() { Id = _userId };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync((UserEntity?)null);
        _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);
        var command = new ConfirmEmailCommand(_userId, token);

        // act

       Func<System.Threading.Tasks.Task> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _userManagerMock.Verify(m => m.ConfirmEmailAsync(user, token), Times.Never);

    }

    [Fact()]
    public async void Handle_EmailNotConfirmed_ShouldThrowBadRequestException()
    {
        // arrange

        var token = HttpUtility.UrlDecode("token");
        var user = new UserEntity() { Id = _userId };
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Failed());
        var command = new ConfirmEmailCommand(_userId, token);

        // act

        Func<System.Threading.Tasks.Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Email could not be confirmed");
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        _userManagerMock.Verify(m => m.ConfirmEmailAsync(user, token), Times.Once);

    }
}