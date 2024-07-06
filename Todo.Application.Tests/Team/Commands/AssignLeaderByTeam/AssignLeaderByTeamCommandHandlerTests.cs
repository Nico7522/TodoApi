using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;
namespace Todo.Application.Team.Commands.AssignLeader.Tests;

public class AssignLeaderByTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Guid _teamId;
    private readonly string _leaderId;
    private readonly AssignLeaderByTeamCommandHandler _handler;
    public AssignLeaderByTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
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
        _leaderId = "leaderId";
        _handler = new AssignLeaderByTeamCommandHandler(_teamRepositoryMock.Object, _userManagerMock.Object);
    }

    [Fact()]
    public async TaskAsync Handle_ForValidCommand_ShouldAssignLeaderAndAddUserToTeam()
    {
        // arrange

        var team = new TeamEntity()
        {
            Id = _teamId,
            LeaderId = null,
            Users = new List<UserEntity>()
        };

        var user = new UserEntity() { Id = _leaderId, TeamId = null };

        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync(user);
        var command = new AssignLeaderByTeamCommand(_teamId, _leaderId);

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _userManagerMock.Verify(m => m.FindByIdAsync(user.Id), Times.Once());
        _teamRepositoryMock.Verify(m => m.SaveChanges(), Times.Once());

        team.LeaderId.Should().Be(user.Id);
        team.Users.Should().HaveCount(1);
        team.Users.First().Id.Should().Be(user.Id);
    }

    [Fact()]
    public async TaskAsync Handle_ForNoActiveTeam_ShouldThrowBadExceptionException()
    {
        // arrange

        var team = new TeamEntity()
        {
            Id = _teamId,
            LeaderId = null,
            Users = new List<UserEntity>(),
            IsActive = false
        };

        var user = new UserEntity() { Id = "id", TeamId = null };

        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync(user);
        var command = new AssignLeaderByTeamCommand(_teamId, _leaderId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Team is not active");

        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _userManagerMock.Verify(m => m.FindByIdAsync(_leaderId), Times.Never());
        _teamRepositoryMock.Verify(m => m.SaveChanges(), Times.Never());

        team.LeaderId.Should().Be(null);
        team.Users.Should().BeEmpty();
        user.TeamId.Should().Be(null);
    }


    [Fact()]
    public async TaskAsync Handle_ForNoExistingUser_ShouldThrowNotFoundException()
    {
        // arrange

        var team = new TeamEntity()
        {
            Id = _teamId,
            LeaderId = null,
            Users = new List<UserEntity>()
        };

        var user = new UserEntity() { Id = "id", TeamId = null };

        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync((UserEntity?)null);
        var command = new AssignLeaderByTeamCommand(_teamId, _leaderId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        var exception = await Assert.ThrowsAsync<NotFoundException>(act);
        Assert.Equal("User not found", exception.Message);

        _teamRepositoryMock.Verify(r => r.GetById(team.Id), Times.Once());
        _userManagerMock.Verify(m => m.FindByIdAsync(_leaderId), Times.Once());
        _teamRepositoryMock.Verify(m => m.SaveChanges(), Times.Never());

        team.LeaderId.Should().Be(null);
        team.Users.Should().BeEmpty();
        user.TeamId.Should().Be(null);  
    }

    [Fact()]
    public async TaskAsync Handle_ForNoExistingTeam_ShouldThrowNotFoundException()
    {
        // arrange

        var team = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            LeaderId = null,
            Users = new List<UserEntity>()
        };

        var user = new UserEntity() { Id = _leaderId, TeamId = null };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync((TeamEntity?)null);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync(user);
        var command = new AssignLeaderByTeamCommand(_teamId, _leaderId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not Found");

        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once());
        _userManagerMock.Verify(m => m.FindByIdAsync(_leaderId), Times.Never());
        _teamRepositoryMock.Verify(m => m.SaveChanges(), Times.Never());

        team.LeaderId.Should().Be(null);
        team.Users.Should().BeEmpty();
        user.TeamId.Should().Be(null);
    }

    [Fact()]
    public async TaskAsync Handle_CommandWithLeaderAlreadyAssigned_ShouldThrowBadException()
    {
        // arrange

        var user = new UserEntity() { Id = _leaderId, TeamId = _teamId };
        var team = new TeamEntity()
        {
            Id = _teamId,
            LeaderId = _leaderId,
            Users = new List<UserEntity>() { user }
        };
        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync(user);
        var command = new AssignLeaderByTeamCommand(_teamId, _leaderId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("User is already leader of this team");
         _userManagerMock.Verify(m => m.FindByIdAsync(user.Id), Times.Once());
        _teamRepositoryMock.Verify(m => m.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(m => m.SaveChanges(), Times.Never());

        team.LeaderId.Should().Be(user.Id);
        team.Users.Should().HaveCount(1);
        team.Users.First().Id.Should().Be(user.Id);
        user.TeamId.Should().Be(team.Id);
    }

    [Fact()]
    public async TaskAsync Handle_CommandWithUserAlreadyInAnotherTeam_ShouldThrowBadException()
    {
        // arrange

        var user = new UserEntity() { Id = _leaderId, TeamId = Guid.NewGuid() };
        var team = new TeamEntity()
        {
            Id = _teamId,
            LeaderId = null,
            Users = new List<UserEntity>() { }
        };
        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync(user);
        var command = new AssignLeaderByTeamCommand(_teamId, _leaderId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("User is already in a team");
        _userManagerMock.Verify(m => m.FindByIdAsync(_leaderId), Times.Once());
        _teamRepositoryMock.Verify(m => m.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(m => m.SaveChanges(), Times.Never());

        team.LeaderId.Should().Be(null);
        team.Users.Should().BeEmpty();
    }

    [Fact()]
    public async TaskAsync Handle_CommandWithTeamAlreadyHasLeader_ShouldThrowBadException()
    {
        // arrange

        var user = new UserEntity() { Id = _leaderId, TeamId = null };
        var team = new TeamEntity()
        {
            Id = _teamId,
            LeaderId = "id",
            Users = new List<UserEntity>() { new UserEntity() }
        };
        _teamRepositoryMock.Setup(r => r.GetById(team.Id)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync(user);
        var command = new AssignLeaderByTeamCommand(_teamId, _leaderId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Team has already a leader");
        _userManagerMock.Verify(m => m.FindByIdAsync(_leaderId), Times.Once());
        _teamRepositoryMock.Verify(m => m.GetById(team.Id), Times.Once());
        _teamRepositoryMock.Verify(m => m.SaveChanges(), Times.Never());

        team.LeaderId.Should().Be("id");
        team.Users.Should().HaveCount(1);
    }
}