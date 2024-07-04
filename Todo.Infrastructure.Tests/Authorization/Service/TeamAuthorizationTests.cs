using Xunit;
using Todo.Infrastructure.Authorization.Service;
using Moq;
using Todo.Application.Users;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Constants;
using FluentAssertions;
using Todo.Domain.Enums;


namespace Todo.Infrastructure.Authorization.Service.Tests;

public class TeamAuthorizationTests
{

    private readonly IAuthorization<TeamEntity> _teamAuthorization;
    private readonly Mock<IUserContext> _userContextMock;
    public TeamAuthorizationTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _teamAuthorization = new TeamAuthorization(_userContextMock.Object);
    }

    [Theory()]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Leader)]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.SuperAdmin)]

    public void Authorize_ForReadOperationWithAllRoles_ShouldReturnTrue(string role)
    {
        // arrange

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", role);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, Domain.Enums.RessourceOperation.Read);

        // assert

        result.Should().Be(true);
    }

    [Theory()]
    [InlineData(RessourceOperation.Read)]
    [InlineData(RessourceOperation.Delete)]
    [InlineData(RessourceOperation.Create)]
    [InlineData(RessourceOperation.Update)]

    public void Authorize_ForAllOperationWithAdminRole_ShouldReturnTrue(RessourceOperation operation)
    {
        // arrange

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", UserRole.Admin);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, operation);

        // assert

        result.Should().Be(true);
    }

    [Theory()]
    [InlineData(RessourceOperation.Read)]
    [InlineData(RessourceOperation.Delete)]
    [InlineData(RessourceOperation.Create)]
    [InlineData(RessourceOperation.Update)]

    public void Authorize_ForAllOperationWithSuperAdminAdminRole_ShouldReturnTrue(RessourceOperation operation)
    {
        // arrange

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", UserRole.Admin);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, operation);

        // assert

        result.Should().Be(true);
    }


    [Fact()]
    public void Authorize_ForCreateOperationWithLeaderRoleAndValidId_ShouldReturnTrue()
    {
        // arrange

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            LeaderId = "id",
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", UserRole.Leader);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, RessourceOperation.Create);

        // assert

        result.Should().Be(true);
    }

    [Fact()]
    public void Authorize_ForCreateOperationWithLeaderRoleAndInvalidId_ShouldReturnFalse()
    {
        // arrange

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            LeaderId = "id2",
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", UserRole.Leader);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, RessourceOperation.Create);

        // assert

        result.Should().Be(false);
    }

    [Fact()]
    public void Authorize_ForCreateOperationWithUserRole_ShouldReturnFalse()
    {
        // arrange

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            LeaderId = "id",
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", UserRole.User);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, RessourceOperation.Create);

        // assert

        result.Should().Be(false);
    }

    [Fact()]
    public void Authorize_ForUpdateOperationWithLeaderRoleAndValidId_ShouldReturnTrue()
    {
        // arrange

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            LeaderId = "id",
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", UserRole.Leader);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, RessourceOperation.Update);

        // assert

        result.Should().Be(true);
    }

    [Fact()]
    public void Authorize_ForUpdateOperationWithLeaderRoleAndInvalidId_ShouldReturnFalse()
    {
        // arrange

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            LeaderId = "id",
        };

        var currentUser = new CurrentUser("id2", "test@gmail.com", UserRole.Leader);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, RessourceOperation.Update);

        // assert

        result.Should().Be(false);
    }

    [Fact()]
    public void Authorize_ForUpdateOperationWithUserRoleAndUserInTeam_ShouldReturnTrue()
    {
        // arrange

        var leader = new UserEntity()
        {
            Id = "id"
        };
        var simpleUser = new UserEntity()
        {
            Id = "id2"
        };

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            LeaderId = "id",
            Users = new List<UserEntity> { leader, simpleUser }
        };

        var currentUser = new CurrentUser("id2", "test@gmail.com", UserRole.User);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, RessourceOperation.Update);

        // assert

        result.Should().Be(true);
    }

    [Fact()]
    public void Authorize_ForUpdateOperationWithUserRoleAndUserNotInTeam_ShouldReturnFalse()
    {
        // arrange

        var leader = new UserEntity()
        {
            Id = "id"
        };
        var simpleUser = new UserEntity()
        {
            Id = "id2"
        };

        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            LeaderId = "id",
            Users = new List<UserEntity> { leader, simpleUser }
        };

        var currentUser = new CurrentUser("id3", "test@gmail.com", UserRole.User);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, RessourceOperation.Update);

        // assert

        result.Should().Be(false);
    }

    [Fact()]
    public void Authorize_ForDeleteOperationWithLeaderRoleAndValidId_ShouldReturnTrue()
    {
        // arrange


        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            LeaderId = "id",
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", UserRole.Leader);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, RessourceOperation.Delete);

        // assert

        result.Should().Be(true);
    }


    [Fact()]
    public void Authorize_ForDeleteOperationWithLeaderRoleAndInalidId_ShouldReturnFalse()
    {
        // arrange


        var teamEntity = new TeamEntity()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            LeaderId = "id",
        };

        var currentUser = new CurrentUser("id2", "test@gmail.com", UserRole.Leader);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _teamAuthorization.Authorize(teamEntity, RessourceOperation.Delete);

        // assert

        result.Should().Be(false);
    }
}