using FluentAssertions;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;
using Moq.EntityFrameworkCore;
using Todo.Application.Tests;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Persistence;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;

namespace Todo.Application.Team.Commands.AddUser.Tests;

public class AssignUserByTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IAuthorization<TeamEntity>> _teamAuthorizationMock;
    private readonly AssignUserByTeamCommandHandler _handler;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Guid _teamId;
    private readonly string _userId;

    public AssignUserByTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _teamAuthorizationMock = new Mock<IAuthorization<TeamEntity>>();    
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
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new AssignUserByTeamCommandHandler(_teamRepositoryMock.Object, _userManagerMock.Object, _teamAuthorizationMock.Object, _userRepositoryMock.Object);
        _teamId = Guid.NewGuid();
        _userId = "userId";
    }


    [Fact()]
    public async TaskAsync Handle_ForValidCommand_ShouldAssignUserCorrectly()
    {
        // arrange
        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "name",
            Users = new List<UserEntity> { }
        };
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = null,
            Team = null,
            FirstName = "name",
            LastName = "name",
        };

        var userList = new List<UserEntity>() { user };
        _userManagerMock.Setup(r => r.Users).Returns(userList.AsQueryable());

        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);
        //_userRepositoryMock.Setup(r => r.GetUserWithTeam(user.Id)).ReturnsAsync(user);
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);

        var command = new AssignUserByTeamCommand(_teamId, _userId);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Once());
        //_userRepositoryMock.Verify(r => r.GetUserWithTeam(user.Id), Times.Once());
        _userManagerMock.Verify(r => r.Users, Times.Once());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Once());
        team.Users.Should().HaveCount(1);
    }

    [Fact()]
    public async TaskAsync Handle_WithNoExistingTeam_ShouldThrowNotFoundException()
    {
        // arrange
        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "name",
            Users = new List<UserEntity> { }
        };
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = null,
            Team = null,
            FirstName = "name",
            LastName = "name",
        };
        var userList = new List<UserEntity>() { user };
        _userManagerMock.Setup(r => r.Users).Returns(userList.AsQueryable());
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);
        //_userRepositoryMock.Setup(r => r.GetUserWithTeam(user.Id)).ReturnsAsync(user);

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync((TeamEntity?)null);

        var command = new AssignUserByTeamCommand(_teamId, _userId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not found");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        //_userRepositoryMock.Verify(r => r.GetUserWithTeam(user.Id), Times.Never());
        _userManagerMock.Verify(r => r.Users, Times.Never());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Never());
        team.Users.Should().HaveCount(0);
    }

    [Fact()]
    public async TaskAsync Handle_ForNoActiveTeam_ShouldThrowBadRequestException()
    {
        // arrange
        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "name",
            Users = new List<UserEntity> { }
        };
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = null,
            Team = null,
            FirstName = "name",
            LastName = "name",
        };
        var userList = new List<UserEntity>() { user };
        _userManagerMock.Setup(r => r.Users).Returns(userList.AsQueryable());
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);
        //_userRepositoryMock.Setup(r => r.GetUserWithTeam(user.Id)).ReturnsAsync(user);

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);

        var command = new AssignUserByTeamCommand(_teamId, _userId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Team is not active");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        //_userRepositoryMock.Verify(r => r.GetUserWithTeam(user.Id), Times.Never());
        _userManagerMock.Verify(r => r.Users, Times.Never());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Never());
        team.Users.Should().HaveCount(0);
    }

    [Fact()]
    public async TaskAsync Handle_WithNoExistingUser_ShouldThrowNotFoundException()
    {
        // arrange
        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "name",
            Users = new List<UserEntity> { }
        };
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = null,
            Team = null,
            FirstName = "name",
            LastName = "name",
        };
        _userManagerMock.Setup(r => r.Users).Throws(new NotFoundException("User not found"));
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);
        //_userRepositoryMock.Setup(r => r.GetUserWithTeam(user.Id)).ReturnsAsync((UserEntity?)null);
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);

        var command = new AssignUserByTeamCommand(_teamId, _userId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        //_userRepositoryMock.Verify(r => r.GetUserWithTeam(user.Id), Times.Once());
        _userManagerMock.Verify(r => r.Users, Times.Once());

        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Never());
        team.Users.Should().HaveCount(0);
    }

    [Fact()]
    public async TaskAsync Handle_ForUserAlreadyInTeam_ShouldThrowBadRequestException()
    {
        // arrange
        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "name",
            Users = new List<UserEntity> { new UserEntity() { Id = _userId } }
        };
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = _teamId,
            Team = team,
            FirstName = "name",
            LastName = "name",
        };
        var userList = new List<UserEntity>() { user };
        _userManagerMock.Setup(r => r.Users).Returns(userList.AsQueryable());
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);
        //_userRepositoryMock.Setup(r => r.GetUserWithTeam(user.Id)).ReturnsAsync(user);
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);

        var command = new AssignUserByTeamCommand(_teamId, _userId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("User already in team");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        //_userRepositoryMock.Verify(r => r.GetUserWithTeam(user.Id), Times.Once());
        _userManagerMock.Verify(r => r.Users, Times.Once());

        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Once());
        team.Users.Should().HaveCount(1);
    }

    [Fact()]
    public async TaskAsync Handle_ForUserAlreadyInAnotherActiveTeam_ShouldThrowBadRequestException()
    {
        // arrange
        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "name",
            Users = new List<UserEntity> {}
        };
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = Guid.NewGuid(),
            FirstName = "name",
            LastName = "name",
            Team = new TeamEntity() { IsActive = true }
        };
        var userList = new List<UserEntity>() { user };
        _userManagerMock.Setup(r => r.Users).Returns(userList.AsQueryable());
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);
        //_userRepositoryMock.Setup(r => r.GetUserWithTeam(user.Id)).ReturnsAsync(user);
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);

        var command = new AssignUserByTeamCommand(_teamId, _userId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("User already in another team");
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never());
        //_userRepositoryMock.Verify(r => r.GetUserWithTeam(user.Id), Times.Once());
        _userManagerMock.Verify(r => r.Users, Times.Once());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Once());
        user.TeamId.Should().NotBe(team.Id);
        user.Team.IsActive.Should().Be(true);
    }

    [Fact()]
    public async TaskAsync Handle_ForValidCommandWithUserInANoActiveTeam_ShouldAssignUserCorrectly()
    {
        // arrange
        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "name",
            Users = new List<UserEntity> { }
        };
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = Guid.NewGuid(),
            FirstName = "name",
            LastName = "name",
            Team = new TeamEntity() { IsActive = false }
        };
        var userList = new List<UserEntity>() { user };
        _userManagerMock.Setup(r => r.Users).Returns(userList.AsQueryable());
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create)).Returns(true);
        //_userRepositoryMock.Setup(r => r.GetUserWithTeam(user.Id)).ReturnsAsync(user);
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);

        var command = new AssignUserByTeamCommand(_teamId, _userId);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert
        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Once());
        //_userRepositoryMock.Verify(r => r.GetUserWithTeam(user.Id), Times.Once());
        _userManagerMock.Verify(r => r.Users, Times.Once());
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Create), Times.Once());
        team.Users.Should().HaveCount(1);
    }


}
