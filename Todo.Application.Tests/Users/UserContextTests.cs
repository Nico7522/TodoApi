using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Todo.Domain.Constants;
using FluentAssertions;
using Todo.Domain.Exceptions;

namespace Todo.Application.Users.Tests
{
    public class UserContextTests
    {
        [Fact()]
        public void GetCurrentUser_WithAuthenticated_ShouldReturnCurrentUser()
        {

            // arrange
            var httpContextAccessotMock = new Mock<IHttpContextAccessor>();
            var claims = new List<Claim>() {
            
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Email, "nico.d@gmail.com"),
                new(ClaimTypes.Role, UserRole.SuperAdmin),

            };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));

            httpContextAccessotMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext() { User = user});

            var userContext = new UserContext(httpContextAccessotMock.Object);

            // act

            var currentUser = userContext.GetCurrentUser();

            // assert
            currentUser.Id.Should().Be("1");
            currentUser.Email.Should().Be("nico.d@gmail.com");
            currentUser.Role.Should().Be(UserRole.SuperAdmin);


        }

        [Fact()]
        public void GetCurrentUser_WithUserContextNotPresent_ShouldThrowsNotFoundException()
        {
            // arrange
            var httpContextAccessotMock = new Mock<IHttpContextAccessor>();

            httpContextAccessotMock.Setup(x => x.HttpContext).Returns((HttpContext?)null);

            var userContext = new UserContext(httpContextAccessotMock.Object);

            // act

            Action action = () => userContext.GetCurrentUser();

            // assert
            action.Should().Throw<NotFoundException>().WithMessage("User not found");
        }
    }
}