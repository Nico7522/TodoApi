using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Web;
using Todo.Application.Email.Interfaces;
using Todo.Domain.Entities;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;


namespace Todo.Application.Users.Commands.ResetPassword.Tests;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly ResetPasswordCommandHandler _handler;
    public ResetPasswordCommandHandlerTests()
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
        _emailSenderMock = new Mock<IEmailSender>();
        _handler = new ResetPasswordCommandHandler(_emailSenderMock.Object, _userManagerMock.Object);
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

        var command = new ResetPasswordCommand("test@gmail.com");
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(entity);
        _userManagerMock.Setup(m => m.GeneratePasswordResetTokenAsync(entity)).ReturnsAsync(resetToken);
        var encodedResetToken = HttpUtility.UrlEncode(resetToken);
        _emailSenderMock.Setup(s => s.SendEmail(entity.Email!, "Reset your password", $"You can reset your password from this url http://localhost:4200/{entity.Id}/{encodedResetToken}", null));

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.GeneratePasswordResetTokenAsync(entity), Times.Once);
        _emailSenderMock.Verify(s => s.SendEmail(entity.Email!, "Reset your password", $"You can reset your password from this url http://localhost:4200/{entity.Id}/{encodedResetToken}", null), Times.Once);
    }
}