using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;


namespace Todo.Application.Users.Commands.UnassignRole.Tests;

public class UnassignRoleCommandHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IUserContext> _userContextMock;
    private readonly string _currentUserId;
    private readonly string _userId;
    private readonly UnassignRoleCommandHandler _handler;
    public UnassignRoleCommandHandlerTests()
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
        _userContextMock = new Mock<IUserContext>();
        _currentUserId = "currentUserId";
        _userId = "userId"; 
        _handler = new UnassignRoleCommandHandler(_userManagerMock.Object, _userContextMock.Object );
    }
    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldUnassignRoleCorrectly()
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = _userId,
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        List<string> userRole = new List<string> { UserRole.Leader };
        List<Claim> userClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.Leader),
        };

        var roleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        var currentUser = new CurrentUser(_currentUserId, "test@gmail.com", UserRole.SuperAdmin);
        var command = new UnassignRoleCommand(_userId);

        _userContextMock.Setup(c => c.GetCurrentUser()).Returns(currentUser);
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync(entity);
        _userManagerMock.Setup(m => m.GetRolesAsync(entity)).ReturnsAsync(userRole);
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(entity, userRole)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetClaimsAsync(entity)).ReturnsAsync(userClaims);
        _userManagerMock.Setup(m => m.RemoveClaimAsync(entity, roleClaim!)).ReturnsAsync(IdentityResult.Success);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _userContextMock.Verify(c => c.GetCurrentUser(), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Once);
        _userManagerMock.Verify(m => m.GetRolesAsync(entity), Times.Once);
        _userManagerMock.Verify(m => m.RemoveFromRolesAsync(entity, userRole), Times.Once);
        _userManagerMock.Verify(m => m.GetClaimsAsync(entity), Times.Once);
        _userManagerMock.Verify(m => m.RemoveClaimAsync(entity, roleClaim!), Times.Once);
    }

    [Fact()]
    public async AsyncTask Handle_ForUserNotFound_ShouldThrowNotFoundException()
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = _userId,
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        List<string> userRole = new List<string> { UserRole.Leader };
        List<Claim> userClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.Leader),
        };

        var roleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        var currentUser = new CurrentUser(_currentUserId, "test@gmail.com", UserRole.SuperAdmin);
        var command = new UnassignRoleCommand(_userId);

        _userContextMock.Setup(c => c.GetCurrentUser()).Returns(currentUser);
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync((UserEntity?)null);
        _userManagerMock.Setup(m => m.GetRolesAsync(entity)).ReturnsAsync(userRole);
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(entity, userRole)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetClaimsAsync(entity)).ReturnsAsync(userClaims);
        _userManagerMock.Setup(m => m.RemoveClaimAsync(entity, roleClaim!)).ReturnsAsync(IdentityResult.Success);

        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        _userContextMock.Verify(c => c.GetCurrentUser(), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Once);
        _userManagerMock.Verify(m => m.GetRolesAsync(entity), Times.Never);
        _userManagerMock.Verify(m => m.RemoveFromRolesAsync(entity, userRole), Times.Never);
        _userManagerMock.Verify(m => m.GetClaimsAsync(entity), Times.Never);
        _userManagerMock.Verify(m => m.RemoveClaimAsync(entity, roleClaim!), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForUserHasNoRole_ShouldThrowBadRequestException()
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = _userId,
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        List<string> userRole = new List<string> { };
        List<Claim> userClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.Leader),
        };

        var roleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        var currentUser = new CurrentUser(_currentUserId, "test@gmail.com", UserRole.SuperAdmin);
        var command = new UnassignRoleCommand(_userId);

        _userContextMock.Setup(c => c.GetCurrentUser()).Returns(currentUser);
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync(entity);
        _userManagerMock.Setup(m => m.GetRolesAsync(entity)).ReturnsAsync(userRole);
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(entity, userRole)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetClaimsAsync(entity)).ReturnsAsync(userClaims);
        _userManagerMock.Setup(m => m.RemoveClaimAsync(entity, roleClaim!)).ReturnsAsync(IdentityResult.Success);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("User has no role");
        _userContextMock.Verify(c => c.GetCurrentUser(), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Once);
        _userManagerMock.Verify(m => m.GetRolesAsync(entity), Times.Once);
        _userManagerMock.Verify(m => m.RemoveFromRolesAsync(entity, userRole), Times.Never);
        _userManagerMock.Verify(m => m.GetClaimsAsync(entity), Times.Never);
        _userManagerMock.Verify(m => m.RemoveClaimAsync(entity, roleClaim!), Times.Never);
    }

    [Theory()]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.SuperAdmin)]

    public async AsyncTask Handle_ForUserTryToRemoveSuperAdminRole_ShouldThrowBadRequestException(string role)
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = _userId,
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        List<string> userRole = new List<string> { UserRole.SuperAdmin };
        List<Claim> userClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.SuperAdmin),
        };

        var roleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        var currentUser = new CurrentUser(_currentUserId, "test@gmail.com", role);
        var command = new UnassignRoleCommand(_userId);

        _userContextMock.Setup(c => c.GetCurrentUser()).Returns(currentUser);
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync(entity);
        _userManagerMock.Setup(m => m.GetRolesAsync(entity)).ReturnsAsync(userRole);
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(entity, userRole)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetClaimsAsync(entity)).ReturnsAsync(userClaims);
        _userManagerMock.Setup(m => m.RemoveClaimAsync(entity, roleClaim!)).ReturnsAsync(IdentityResult.Success);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("You can't achieve this action");
        _userContextMock.Verify(c => c.GetCurrentUser(), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Once);
        _userManagerMock.Verify(m => m.GetRolesAsync(entity), Times.Once);
        _userManagerMock.Verify(m => m.RemoveFromRolesAsync(entity, userRole), Times.Never);
        _userManagerMock.Verify(m => m.GetClaimsAsync(entity), Times.Never);
        _userManagerMock.Verify(m => m.RemoveClaimAsync(entity, roleClaim!), Times.Never);
    }


    [Fact()]
    public async AsyncTask Handle_ForUserWithAdminRoleTryToRemoveAdminRole_ShouldThrowBadRequestException()
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = _userId,
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        List<string> userRole = new List<string> { UserRole.Admin };
        List<Claim> userClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.Admin),
        };

        var roleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        var currentUser = new CurrentUser(_currentUserId, "test@gmail.com", UserRole.Admin);
        var command = new UnassignRoleCommand(_userId);

        _userContextMock.Setup(c => c.GetCurrentUser()).Returns(currentUser);
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync(entity);
        _userManagerMock.Setup(m => m.GetRolesAsync(entity)).ReturnsAsync(userRole);
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(entity, userRole)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetClaimsAsync(entity)).ReturnsAsync(userClaims);
        _userManagerMock.Setup(m => m.RemoveClaimAsync(entity, roleClaim!)).ReturnsAsync(IdentityResult.Success);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("You can't achieve this action");
        _userContextMock.Verify(c => c.GetCurrentUser(), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Once);
        _userManagerMock.Verify(m => m.GetRolesAsync(entity), Times.Once);
        _userManagerMock.Verify(m => m.RemoveFromRolesAsync(entity, userRole), Times.Never);
        _userManagerMock.Verify(m => m.GetClaimsAsync(entity), Times.Never);
        _userManagerMock.Verify(m => m.RemoveClaimAsync(entity, roleClaim!), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForRoleNotRemoved_ShouldThrowApiException()
    {
        // arrange
        var entity = new UserEntity()
        {
            Id = _userId,
            FirstName = "test",
            LastName = "test",
            Email = "test@gmail.com",
            PasswordHash = "qdqsd4454qsdqd",
            PhoneNumber = "491414141",
            Birthdate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2010, 01, 01),
        };
        List<string> userRole = new List<string> { UserRole.Leader };
        List<Claim> userClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.Leader),
        };

        var roleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

        var currentUser = new CurrentUser(_currentUserId, "test@gmail.com", UserRole.SuperAdmin);
        var command = new UnassignRoleCommand(_userId);

        _userContextMock.Setup(c => c.GetCurrentUser()).Returns(currentUser);
        _userManagerMock.Setup(m => m.FindByIdAsync(command.UserId)).ReturnsAsync(entity);
        _userManagerMock.Setup(m => m.GetRolesAsync(entity)).ReturnsAsync(userRole);
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(entity, userRole)).ReturnsAsync(IdentityResult.Failed());
        _userManagerMock.Setup(m => m.GetClaimsAsync(entity)).ReturnsAsync(userClaims);
        _userManagerMock.Setup(m => m.RemoveClaimAsync(entity, roleClaim!)).ReturnsAsync(IdentityResult.Success);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<ApiException>().WithMessage("Role has not been deleted");
        _userContextMock.Verify(c => c.GetCurrentUser(), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(command.UserId), Times.Once);
        _userManagerMock.Verify(m => m.GetRolesAsync(entity), Times.Once);
        _userManagerMock.Verify(m => m.RemoveFromRolesAsync(entity, userRole), Times.Once);
        _userManagerMock.Verify(m => m.GetClaimsAsync(entity), Times.Never);
        _userManagerMock.Verify(m => m.RemoveClaimAsync(entity, roleClaim!), Times.Never);
    }
}