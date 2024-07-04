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
        team.LeaderId.Should().Be(user.Id);
        team.Users.Should().HaveCount(1);
        team.Users.First().Id.Should().Be(user.Id);
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

        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Never());
        _userManagerMock.Verify(m => m.FindByIdAsync(user.Id), Times.Never());
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not Found");
        team.LeaderId.Should().Be(null);
        team.Users.Should().BeEmpty();
        user.TeamId.Should().Be(null);  
    }
}