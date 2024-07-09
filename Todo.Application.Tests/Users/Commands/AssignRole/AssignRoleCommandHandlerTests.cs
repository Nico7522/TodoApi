using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;



namespace Todo.Application.Users.Commands.AssignRole.Tests;

public class AssignRoleCommandHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IValidator<AssignRoleCommand>> _validatorMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessotMock;
    private readonly AssignRoleCommandHandler _handler;

    public AssignRoleCommandHandlerTests()
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
        _validatorMock = new Mock<IValidator<AssignRoleCommand>>();
        _userContextMock = new Mock<IUserContext>();
        _httpContextAccessotMock = new Mock<IHttpContextAccessor>();
        _handler = new AssignRoleCommandHandler(_userManagerMock.Object, _validatorMock.Object, _userContextMock.Object);
    }
    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldAssignRoleCorrectly()
    {
        // arrange
        var userId = "userId";
        var command = new AssignRoleCommand(userId, "Leader");
        _validatorMock.Setup(v => v.Validate(command)).Returns(new FluentValidation.Results.ValidationResult());
        var userToUpdate = new UserEntity()
        {
            Id = userId
        };
        var claims = new List<Claim>() {

                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Email, "nico.d@gmail.com"),
                new(ClaimTypes.Role, UserRole.User),

            };
        var userRole = new List<string> { UserRole.User };
        var currentUser = new CurrentUser("id", "test@gmail.com", UserRole.SuperAdmin);
        _userContextMock.Setup(r => r.GetCurrentUser()).Returns(currentUser);
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync(userToUpdate);
        _userManagerMock.Setup(m => m.GetRolesAsync(userToUpdate)).ReturnsAsync(userRole);
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(userToUpdate, userRole)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(userToUpdate, command.Role)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetClaimsAsync(userToUpdate)).ReturnsAsync(claims);
        _userManagerMock.Setup(m => m.ReplaceClaimAsync(userToUpdate, It.IsAny<Claim>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);
        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert
        _validatorMock.Verify(v => v.Validate(command), Times.Once);

        //_userManagerMock.Verify(r => r.ReplaceClaimAsync(userToUpdate, It.IsAny<Claim>(), It.IsAny<Claim>()), Times.Once);

    }
}