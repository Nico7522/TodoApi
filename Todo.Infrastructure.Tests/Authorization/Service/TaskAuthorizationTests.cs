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
    [Fact()]
    public void Authorize_ForAuthorizedUserAction_ShouldReturnTrue()
    {
        // arrange

        var todoEntity = new TodoEntity()
        {
            Id = Guid.NewGuid(),    
            Title = "Test",
            Description = "Test",
            UserId = "id"
        };

        var currentUser = new CurrentUser("id", "test@gmail.com", UserRole.User);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _todoAuthorization.Authorize(todoEntity, Domain.Enums.RessourceOperation.Update);

        // assert

        result.Should().Be(true);
    }

    [Fact()]
    public void Authorize_ForUnauthorizedUserAction_ShouldReturnFalse()
    {
        // arrange

        var todoEntity = new TodoEntity()
        {
            Id = Guid.NewGuid(),
            Title = "Test",
            Description = "Test",
            UserId = "id"
        };

        var currentUser = new CurrentUser("id2", "test@gmail.com", UserRole.User);
        _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        // act

        var result = _todoAuthorization.Authorize(todoEntity, Domain.Enums.RessourceOperation.Update);

        // assert

        result.Should().Be(false);
    }
}