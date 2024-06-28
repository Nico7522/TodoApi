using FluentAssertions;
using Todo.Domain.Constants;
using Xunit;

namespace Todo.Application.Users.Tests;

public class CurrentUserTests
{
    [Fact()]
    public void HasRole_WithMatchingRole_ShouldReturnTrue()
    {
        // arrange

        var currentUser = new CurrentUser("1", "test@gmail.com", UserRole.SuperAdmin); 

        // act

        var hasRole = currentUser.HasRole(UserRole.SuperAdmin);

        // assert

        hasRole.Should().BeTrue();
    }

    [Theory()]
    [InlineData(UserRole.User)]
    [InlineData(UserRole.Admin)]
    [InlineData(UserRole.Leader)]
    public void HasRole_WithNoMatchingRole_ShouldReturnFalse(string role)
    {
        // arrange

        var currentUser = new CurrentUser("1", "test@gmail.com", UserRole.SuperAdmin);

        // act

        var hasRole = currentUser.HasRole(role);

        // assert

        hasRole.Should().BeFalse();
    }

}