using Xunit;
using Moq;
using Todo.Application.Users;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Constants;
using FluentAssertions;

namespace Todo.Infrastructure.Authorization.Service.Tests;

public class TaskAuthorizationTests
{
    private readonly IAuthorization<TodoEntity> _todoAuthorization;
    private readonly Mock<IUserContext> _userContextMock;
    public TaskAuthorizationTests()
    {
        _userContextMock = new Mock<IUserContext>();
        _todoAuthorization = new TaskAuthorization(_userContextMock.Object);
    }

    [Theory()]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Leader)]
    public void Authorize_ForAuthorizedUserOrLeaderAction_ShouldReturnTrue(string role)
    {
        // arrange

        var todoEntity = new TodoEntity()
        {
            Id = Guid.NewGuid(),    
            Title = "Test",
            Description = "Test",
            UserId = "id"
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", role);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _todoAuthorization.Authorize(todoEntity, Domain.Enums.RessourceOperation.Update);

        // assert
        
        result.Should().Be(true);
    }

    [Theory()]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Leader)]
    public void Authorize_ForUnauthorizedUserOrLeaderAction_ShouldReturnFalse(string role)
    {
        // arrange

        var todoEntity = new TodoEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Description = "Test",
            UserId = "id"
        };

        var currentUser = new CurrentUser("id2", "test@gmail.com", role);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _todoAuthorization.Authorize(todoEntity, Domain.Enums.RessourceOperation.Update);

        // assert

        result.Should().Be(false);
    }

    [Theory()]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.SuperAdmin)]
    public void Authorize_ForAuthorizedAdminAndSuperAdminAction_ShouldReturnTrue(string role)
    {
        // arrange

        var todoEntity = new TodoEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Description = "Test",
            UserId = "id"
        };

        var currentUser = new CurrentUser("id2", "test@gmail.com", role);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _todoAuthorization.Authorize(todoEntity, Domain.Enums.RessourceOperation.Update);

        // assert

        result.Should().Be(true);
    }


}