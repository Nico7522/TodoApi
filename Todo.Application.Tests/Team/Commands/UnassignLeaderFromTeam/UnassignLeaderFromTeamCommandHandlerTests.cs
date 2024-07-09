using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;

namespace Todo.Application.Team.Commands.UnassignLeader.Tests;

public class UnassignLeaderFromTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly UnassignLeaderFromTeamCommandHandler _handler;
    private readonly Guid _teamId;

    private readonly string _leaderId;
    public UnassignLeaderFromTeamCommandHandlerTests()
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
        _handler = new UnassignLeaderFromTeamCommandHandler(_teamRepositoryMock.Object, _userManagerMock.Object);
        _teamId = Guid.NewGuid();
        _leaderId = "id";
    }
    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldUnassignLeaderCorrectly()
    {
        // arrange
        var leader = new UserEntity()
        {
            Id = _leaderId
        };

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            Leader = leader,
            LeaderId = _leaderId,
            Users = new List<UserEntity>() { leader }
        };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync(leader);

        var command = new UnassignLeaderFromTeamCommand(_teamId);
        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        _userManagerMock.Verify(m => m.FindByIdAsync(_leaderId), Times.Once);
        team.Leader.Should().Be(null);
        team.Users.Should().BeEmpty();   
    }

    [Fact()]
    public async AsyncTask Handle_ForNoExistingTeam_ShouldThrowNotFoundException()
    {
        // arrange
        var leader = new UserEntity()
        {
            Id = _leaderId
        };

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            Leader = leader,
            LeaderId = _leaderId,
            Users = new List<UserEntity>() { leader }
        };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync((TeamEntity?)null);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync(leader);

        var command = new UnassignLeaderFromTeamCommand(_teamId);
        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not found");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _userManagerMock.Verify(m => m.FindByIdAsync(_leaderId), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForNoExistingTeamLeader_ShouldThrowNotFoundException()
    {
        // arrange
        var leader = new UserEntity()
        {
            Id = _leaderId
        };

        var team = new TeamEntity()
        {
            Id = _teamId,
            Name = "Test",
            Leader = leader,
            LeaderId = _leaderId,
            Users = new List<UserEntity>() { leader }
        };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _userManagerMock.Setup(m => m.FindByIdAsync(_leaderId)).ReturnsAsync((UserEntity?)null);

        var command = new UnassignLeaderFromTeamCommand(_teamId);
        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team leader not found");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _teamRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _userManagerMock.Verify(m => m.FindByIdAsync(_leaderId), Times.Once);
    }
}