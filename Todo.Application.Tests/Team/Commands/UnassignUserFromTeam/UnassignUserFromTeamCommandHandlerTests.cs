using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;


namespace Todo.Application.Team.Commands.DeleteUser.Tests;

public class UnassignUserFromTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IAuthorization<TeamEntity>> _teamAuthorizationMock;
    private readonly Guid _teamId;
    private readonly string _userId;
    private readonly UnassignUserFromTeamCommandHandler _handler;
    public UnassignUserFromTeamCommandHandlerTests()
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
        _teamId = Guid.NewGuid();
        _userId = "id";
        _handler = new UnassignUserFromTeamCommandHandler(_teamRepositoryMock.Object, _userManagerMock.Object, _teamAuthorizationMock.Object);
    }

    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldUnassignUserCorrectly()
    {
        // arrange
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = _teamId,
        };

        var team = new TeamEntity { Id = _teamId, Name = "test", Users = new List<UserEntity> { user } };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var command = new UnassignUserFromTeamCommand(_teamId, _userId);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        team.Users.Should().BeEmpty();
    }

    [Fact()]
    public async AsyncTask Handle_ForNoExistingTeam_ShouldThrowNotFoundException()
    {
        // arrange
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = _teamId,
        };

        var team = new TeamEntity { Id = _teamId, Name = "test", Users = new List<UserEntity> { user } };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync((TeamEntity?)null);
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var command = new UnassignUserFromTeamCommand(_teamId, _userId);

        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not found");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Never);
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForNoExistingUser_ShouldThrowNotFoundException()
    {
        // arrange
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = _teamId,
        };

        var team = new TeamEntity { Id = _teamId, Name = "test", Users = new List<UserEntity> { user } };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync((UserEntity?)null);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var command = new UnassignUserFromTeamCommand(_teamId, _userId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Never);
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
    }

    [Fact()]
    public async AsyncTask Handle_ForUnauthorizedUserAction_ShouldThrowForbidException()
    {
        // arrange
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = _teamId,
        };

        var team = new TeamEntity { Id = _teamId, Name = "test", Users = new List<UserEntity> { user } };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(false);
        var command = new UnassignUserFromTeamCommand(_teamId, _userId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<ForbidException>().WithMessage("Your not authorized");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        team.Users.Should().HaveCount(1);
    }

    [Fact()]
    public async AsyncTask Handle_ForUserIsAlsoTeamLeader_ShouldBadRequestException()
    {
        // arrange
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = _teamId,
        };

        var team = new TeamEntity { Id = _teamId, LeaderId = _userId, Name = "test", Users = new List<UserEntity> { user } };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var command = new UnassignUserFromTeamCommand(_teamId, _userId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<BadRequestException>().WithMessage("You cannot remove yourself from your team");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        team.Users.Should().HaveCount(1);
    }

    [Fact()]
    public async AsyncTask Handle_ForUserNotInTeam_ShouldBadRequestException()
    {
        // arrange
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = null,
        };

        var team = new TeamEntity { Id = _teamId, Name = "test", Users = new List<UserEntity> { user } };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_userId)).ReturnsAsync(user);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var command = new UnassignUserFromTeamCommand(_teamId, _userId);

        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<BadRequestException>().WithMessage("User is not in team");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Delete), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(_userId), Times.Once);
        team.Users.Should().HaveCount(1);
    }
}