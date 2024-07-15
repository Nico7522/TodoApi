using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;


namespace Todo.Application.Users.Queries.GetTasksNumberByUser.Tests;

public class GetTasksNumberByUserQueryHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly GetTasksNumberByUserQueryHandler _handler;

    public GetTasksNumberByUserQueryHandlerTests()
    {
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
        _handler = new GetTasksNumberByUserQueryHandler(_userManagerMock.Object);
    }
    //[Fact()]
    //public async void Handle_ForValidQuery_ShouldReturnTaskNumberCorrectly()
    //{
    //    // arrange
    //    string userId = "userId";
    //    var user = new UserEntity() { Id = userId, Tasks = new List<TodoEntity> { new TodoEntity() } };
    //    var userList = new List<UserEntity> { user };
    //    var command = new GetTasksNumberByUserQuery(userId);
    //    _userManagerMock.Setup(m => m.Users).Returns(userList.AsQueryable());

    //    // act

    //    await _handler.Handle(command, CancellationToken.None);

    //    // assert

    //    _userManagerMock.Verify(m => m.Users, Times.Once);

    //}

    //[Fact()]
    //public async void Handle_UserNotFound_ShouldThrowNotFoundException()
    //{
    //    // arrange
    //    string userId = "userId";
    //    var user = new UserEntity() { Id = userId, Tasks = new List<TodoEntity> { new TodoEntity() } };
    //    var userList = new List<UserEntity> { user };
    //    var command = new GetTasksNumberByUserQuery(userId);
    //    _userManagerMock.Setup(m => m.Users).Throws(new NotFoundException("user not found"));

    //    // act

    //    Func<TaskAsync> act = async() => await _handler.Handle(command, CancellationToken.None);

    //    // assert
    //    await act.Should().ThrowAsync<NotFoundException>();
    //    _userManagerMock.Verify(m => m.Users, Times.Once);

    //}
}