using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;
using Todo.Application.Email.Interfaces;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Security;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;


namespace Todo.Application.Users.Commands.Register.Tests;

public class RegisterCommandHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IJwtHelper> _jwtHelperMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<IEmailService> _emailServiceMock;

    private readonly RegisterCommandHandler _handler;
    public RegisterCommandHandlerTests()
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
        _mapperMock = new Mock<IMapper>();
        _jwtHelperMock = new Mock<IJwtHelper>();
        _emailSenderMock = new Mock<IEmailSender>();
        _emailServiceMock = new Mock<IEmailService>();
        _handler = new RegisterCommandHandler(_mapperMock.Object, _userManagerMock.Object, _jwtHelperMock.Object, _emailSenderMock.Object, _emailServiceMock.Object);
    }
    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldRegisterUserCorrectly()
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
        var command = new RegisterCommand();
        command.Email = "test@gmail.com";
        command.FirstName = "test";
        command.LastName = "test";
        command.Password = "@Test12345";
        command.PasswordConfirm = "@Test12345";
        command.PhoneNumber = "491414141";
        command.BirthDate = new DateOnly(2000, 01, 01);
        command.HireDate = new DateOnly(2010, 01, 01);
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.User),
        };
        _mapperMock.Setup(m => m.Map<UserEntity>(command)).Returns(entity);
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((UserEntity?)null);
        _userManagerMock.Setup(m => m.CreateAsync(entity, command.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(entity, UserRole.User)).ReturnsAsync(IdentityResult.Success);
        _jwtHelperMock.Setup(h => h.SetBaseClaims(entity)).ReturnsAsync(claims);
        _userManagerMock.Setup(m => m.AddClaimsAsync(entity, claims)).ReturnsAsync(IdentityResult.Success);
        _emailSenderMock.Setup(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null));


        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert
        _mapperMock.Verify(m => m.Map<UserEntity>(command), Times.Once);
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CreateAsync(entity, command.Password), Times.Once);
        _userManagerMock.Verify(m => m.AddToRoleAsync(entity, UserRole.User), Times.Once);
        _jwtHelperMock.Verify(h => h.SetBaseClaims(entity), Times.Once);
        _userManagerMock.Verify(m => m.AddClaimsAsync(entity, claims), Times.Once);
        _emailSenderMock.Verify(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null), Times.Once);
    }

    [Fact()]
    public async AsyncTask Handle_ForEmailAlreadyUsed_ShouldThrowBadRequestException()
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
        var command = new RegisterCommand();
        command.Email = "test@gmail.com";
        command.FirstName = "test";
        command.LastName = "test";
        command.Password = "@Test12345";
        command.PasswordConfirm = "@Test12345";
        command.PhoneNumber = "491414141";
        command.BirthDate = new DateOnly(2000, 01, 01);
        command.HireDate = new DateOnly(2010, 01, 01);
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.User),
        };
        _mapperMock.Setup(m => m.Map<UserEntity>(command)).Returns(entity);
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync(entity);
        _userManagerMock.Setup(m => m.CreateAsync(entity, command.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(entity, UserRole.User)).ReturnsAsync(IdentityResult.Success);
        _jwtHelperMock.Setup(h => h.SetBaseClaims(entity)).ReturnsAsync(claims);
        _userManagerMock.Setup(m => m.AddClaimsAsync(entity, claims)).ReturnsAsync(IdentityResult.Success);
        _emailSenderMock.Setup(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null));


        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Bad request");
        _mapperMock.Verify(m => m.Map<UserEntity>(command), Times.Once);
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CreateAsync(entity, command.Password), Times.Never);
        _userManagerMock.Verify(m => m.AddToRoleAsync(entity, UserRole.User), Times.Never);
        _jwtHelperMock.Verify(h => h.SetBaseClaims(entity), Times.Never);
        _userManagerMock.Verify(m => m.AddClaimsAsync(entity, claims), Times.Never);
        _emailSenderMock.Verify(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForUserNotCreated_ShouldThrowApiException()
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
        var command = new RegisterCommand();
        command.Email = "test@gmail.com";
        command.FirstName = "test";
        command.LastName = "test";
        command.Password = "@Test12345";
        command.PasswordConfirm = "@Test12345";
        command.PhoneNumber = "491414141";
        command.BirthDate = new DateOnly(2000, 01, 01);
        command.HireDate = new DateOnly(2010, 01, 01);
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.User),
        };
        _mapperMock.Setup(m => m.Map<UserEntity>(command)).Returns(entity);
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((UserEntity?)null);
        _userManagerMock.Setup(m => m.CreateAsync(entity, command.Password)).ReturnsAsync(IdentityResult.Failed());
        _userManagerMock.Setup(m => m.AddToRoleAsync(entity, UserRole.User)).ReturnsAsync(IdentityResult.Success);
        _jwtHelperMock.Setup(h => h.SetBaseClaims(entity)).ReturnsAsync(claims);
        _userManagerMock.Setup(m => m.AddClaimsAsync(entity, claims)).ReturnsAsync(IdentityResult.Success);
        _emailSenderMock.Setup(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null));


        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<ApiException>().WithMessage("An error has occurred");
        _mapperMock.Verify(m => m.Map<UserEntity>(command), Times.Once);
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CreateAsync(entity, command.Password), Times.Once);
        _userManagerMock.Verify(m => m.AddToRoleAsync(entity, UserRole.User), Times.Never);
        _jwtHelperMock.Verify(h => h.SetBaseClaims(entity), Times.Never);
        _userManagerMock.Verify(m => m.AddClaimsAsync(entity, claims), Times.Never);
        _emailSenderMock.Verify(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForRoleNotAdded_ShouldThrowApiException()
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
        var command = new RegisterCommand();
        command.Email = "test@gmail.com";
        command.FirstName = "test";
        command.LastName = "test";
        command.Password = "@Test12345";
        command.PasswordConfirm = "@Test12345";
        command.PhoneNumber = "491414141";
        command.BirthDate = new DateOnly(2000, 01, 01);
        command.HireDate = new DateOnly(2010, 01, 01);
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.User),
        };
        _mapperMock.Setup(m => m.Map<UserEntity>(command)).Returns(entity);
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((UserEntity?)null);
        _userManagerMock.Setup(m => m.CreateAsync(entity, command.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(entity, UserRole.User)).ReturnsAsync(IdentityResult.Failed());
        _jwtHelperMock.Setup(h => h.SetBaseClaims(entity)).ReturnsAsync(claims);
        _userManagerMock.Setup(m => m.AddClaimsAsync(entity, claims)).ReturnsAsync(IdentityResult.Success);
        _emailSenderMock.Setup(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null));


        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<ApiException>().WithMessage("An error has occurred");
        _mapperMock.Verify(m => m.Map<UserEntity>(command), Times.Once);
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CreateAsync(entity, command.Password), Times.Once);
        _userManagerMock.Verify(m => m.AddToRoleAsync(entity, UserRole.User), Times.Once);
        _jwtHelperMock.Verify(h => h.SetBaseClaims(entity), Times.Never);
        _userManagerMock.Verify(m => m.AddClaimsAsync(entity, claims), Times.Never);
        _emailSenderMock.Verify(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForClaimsNotAdded_ShouldThrowApiException()
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
        var command = new RegisterCommand();
        command.Email = "test@gmail.com";
        command.FirstName = "test";
        command.LastName = "test";
        command.Password = "@Test12345";
        command.PasswordConfirm = "@Test12345";
        command.PhoneNumber = "491414141";
        command.BirthDate = new DateOnly(2000, 01, 01);
        command.HireDate = new DateOnly(2010, 01, 01);
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, entity.Id),
            new Claim(ClaimTypes.Email, entity.Email!),
            new Claim(ClaimTypes.Role, UserRole.User),
        };
        _mapperMock.Setup(m => m.Map<UserEntity>(command)).Returns(entity);
        _userManagerMock.Setup(m => m.FindByEmailAsync(command.Email)).ReturnsAsync((UserEntity?)null);
        _userManagerMock.Setup(m => m.CreateAsync(entity, command.Password)).ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(entity, UserRole.User)).ReturnsAsync(IdentityResult.Success);
        _jwtHelperMock.Setup(h => h.SetBaseClaims(entity)).ReturnsAsync(claims);
        _userManagerMock.Setup(m => m.AddClaimsAsync(entity, claims)).ReturnsAsync(IdentityResult.Failed());
        _emailSenderMock.Setup(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null));


        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<ApiException>().WithMessage("An error has occurred");
        _mapperMock.Verify(m => m.Map<UserEntity>(command), Times.Once);
        _userManagerMock.Verify(m => m.FindByEmailAsync(command.Email), Times.Once);
        _userManagerMock.Verify(m => m.CreateAsync(entity, command.Password), Times.Once);
        _userManagerMock.Verify(m => m.AddToRoleAsync(entity, UserRole.User), Times.Once);
        _jwtHelperMock.Verify(h => h.SetBaseClaims(entity), Times.Once);
        _userManagerMock.Verify(m => m.AddClaimsAsync(entity, claims), Times.Once);
        _emailSenderMock.Verify(s => s.SendEmail(entity.Email!, "Account created", $"Welcome, to our team {entity.FirstName} {entity.LastName}", null), Times.Never);
    }
}