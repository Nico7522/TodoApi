using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Security;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;

namespace Todo.Application.Users.Commands.Login.Tests;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IJwtHelper> _jwtHelperMock;
    private readonly LoginCommandHandler _handler;
    private readonly string _userId;
    public LoginCommandHandlerTests()
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
        _jwtHelperMock = new Mock<IJwtHelper>();
        _handler = new LoginCommandHandler(_userManagerMock.Object, _jwtHelperMock.Object);
        _userId = "userId";
    }
    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldLoginUserCorrectly()
    {
        var user = new UserEntity
        {
            Id = _userId,
            Email = "test@gmail.com",
            PasswordHash = "dqd45qd4q5sd4qsd45"
        };
        // arrange
        var command = new LoginCommand();
        command.Email = "test@gmail.com";
        command.Password = "@Test12345";

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, command.Password)).ReturnsAsync(true);
        _jwtHelperMock.Setup(h => h.GenerateToken(user)).ReturnsAsync("token");


        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CheckPasswordAsync(user, command.Password), Times.Once);
        _jwtHelperMock.Verify(h => h.GenerateToken(user), Times.Once);

    }

    [Fact()]
    public async AsyncTask Handle_ForBadEmail_ShouldThrowBadRequestException()
    {
        var user = new UserEntity
        {
            Id = _userId,
            Email = "test@gmail.com",
            PasswordHash = "dqd45qd4q5sd4qsd45"
        };
        // arrange
        var command = new LoginCommand();
        command.Email = "test@gmail.com";
        command.Password = "@Test12345";

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((UserEntity?)null);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, command.Password)).ReturnsAsync(true);
        _jwtHelperMock.Setup(h => h.GenerateToken(user)).ReturnsAsync("token");

        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Bad credentials");
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CheckPasswordAsync(user, command.Password), Times.Never);
        _jwtHelperMock.Verify(h => h.GenerateToken(user), Times.Never);
    }


    [Fact()]
    public async AsyncTask Handle_ForBadPassword_ShouldThrowBadRequestException()
    {
        var user = new UserEntity
        {
            Id = _userId,
            Email = "test@gmail.com",
            PasswordHash = "dqd45qd4q5sd4qsd45"
        };
        // arrange
        var command = new LoginCommand();
        command.Email = "test@gmail.com";
        command.Password = "@Test12345";

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, command.Password)).ReturnsAsync(false);
        _jwtHelperMock.Setup(h => h.GenerateToken(user)).ReturnsAsync("token");

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Bad credentials");
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CheckPasswordAsync(user, command.Password), Times.Once);
        _jwtHelperMock.Verify(h => h.GenerateToken(user), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForBadAccountNotConfirmed_ShouldThrowBadRequestException()
    {
        var user = new UserEntity
        {
            Id = _userId,
            Email = "test@gmail.com",
            PasswordHash = "dqd45qd4q5sd4qsd45",
            EmailConfirmed = false,
           
        };
        // arrange
        var command = new LoginCommand();
        command.Email = "test@gmail.com";
        command.Password = "@Test12345";

        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, command.Password)).ReturnsAsync(true);
        _jwtHelperMock.Setup(h => h.GenerateToken(user)).ReturnsAsync("token");

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Account not confirmed");
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CheckPasswordAsync(user, command.Password), Times.Never);
        _jwtHelperMock.Verify(h => h.GenerateToken(user), Times.Never);
    }
}