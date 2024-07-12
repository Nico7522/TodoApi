using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Web;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;


namespace Todo.Application.Users.Commands.ResetPasswordConfirm.Tests;

public class ResetPasswordConfirmCommandHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IValidator<ResetPasswordConfirmCommand>> _validatorMock;
    private readonly ResetPasswordConfirmCommandHandler _handler;
    public ResetPasswordConfirmCommandHandlerTests()
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
        _validatorMock = new Mock<IValidator<ResetPasswordConfirmCommand>>();
        _handler = new ResetPasswordConfirmCommandHandler(_userManagerMock.Object, _validatorMock.Object);
    }

    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldResetPasswordCorrectly()
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = "userId",
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        var resetToken = "resetToken";
        var encodedResetToken = HttpUtility.UrlEncode(resetToken);
        var command = new ResetPasswordConfirmCommand("userId", encodedResetToken, "test@12345", "test@12345");
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync(entity);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _userManagerMock.Setup(m => m.ResetPasswordAsync(entity, encodedResetToken, command.Password)).ReturnsAsync(IdentityResult.Success);
        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Once);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _userManagerMock.Verify(m => m.ResetPasswordAsync(entity, encodedResetToken, command.Password), Times.Once);
    }

    [Fact()]
    public async AsyncTask Handle_ForUnvalidCommand_ShouldThrowBadRequestException()
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = "userId",
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        var resetToken = "resetToken";
        var encodedResetToken = HttpUtility.UrlEncode(resetToken);
        var command = new ResetPasswordConfirmCommand("userId", encodedResetToken, "test@12345", "test@12345");
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync(entity);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<ValidationFailure>() { new ValidationFailure("field", "error")}));
        _userManagerMock.Setup(m => m.ResetPasswordAsync(entity, encodedResetToken, command.Password)).ReturnsAsync(IdentityResult.Success);
        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<ValidationException>();
        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Never);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _userManagerMock.Verify(m => m.ResetPasswordAsync(entity, encodedResetToken, command.Password), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForUserNotFound_ShouldThrowNotFoundException()
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = "userId",
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        var resetToken = "resetToken";
        var encodedResetToken = HttpUtility.UrlEncode(resetToken);
        var command = new ResetPasswordConfirmCommand("userId", encodedResetToken, "test@12345", "test@12345");
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync((UserEntity?)null);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _userManagerMock.Setup(m => m.ResetPasswordAsync(entity, encodedResetToken, command.Password)).ReturnsAsync(IdentityResult.Success);
        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Once);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _userManagerMock.Verify(m => m.ResetPasswordAsync(entity, encodedResetToken, command.Password), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_PasswordNotReset_ShouldThrowApiExceptionException()
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = "userId",
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        var resetToken = "resetToken";
        var encodedResetToken = HttpUtility.UrlEncode(resetToken);
        var command = new ResetPasswordConfirmCommand("userId", encodedResetToken, "test@12345", "test@12345");
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync(entity);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _userManagerMock.Setup(m => m.ResetPasswordAsync(entity, encodedResetToken, command.Password)).ReturnsAsync(IdentityResult.Failed());
        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<ApiException>().WithMessage("A error has occured");
        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Once);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _userManagerMock.Verify(m => m.ResetPasswordAsync(entity, encodedResetToken, command.Password), Times.Once);
    }
}

