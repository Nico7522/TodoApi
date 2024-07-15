using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;

namespace Todo.Application.Users.Queries.GetUserById.Tests;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetUserByIdQueryHandler _handler;
    public GetUserByIdQueryHandlerTests()
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
        _mapperMock = new Mock<IMapper>();
        _handler = new GetUserByIdQueryHandler(_userManagerMock.Object, _mapperMock.Object);   
    }
    [Fact()]
    public async TaskAsync Handle_ForValidQuery_ShouldReturnUserCorrectly()
    {
        // arrange
        var userDto = new UserDto();
        string userId = "userId";
        var user = new UserEntity() { Id = userId};
        var userList = new List<UserEntity> { user };
        var command = new GetUserByIdQuery(userId);
        _userManagerMock.Setup(m => m.Users).Returns(userList.AsQueryable());
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _userManagerMock.Verify(m => m.Users, Times.Once);
        _mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);

    }

    [Fact()]
    public async TaskAsync Handle_ForUseNotFound_ShouldThrowNotFoundException()
    {
        // arrange
        var userDto = new UserDto();
        string userId = "userId";
        var user = new UserEntity() { Id = userId };
        var userList = new List<UserEntity> { user };
        var command = new GetUserByIdQuery(userId);
        _userManagerMock.Setup(m => m.Users).Throws(new NotFoundException("User not found"));
        _mapperMock.Setup(m => m.Map<UserDto>(user)).Returns(userDto);
        // act

        Func<TaskAsync> act = async() => await _handler.Handle(command, CancellationToken.None);

        // assert
        await act.Should().ThrowAsync<NotFoundException>();
        _userManagerMock.Verify(m => m.Users, Times.Once);
        _mapperMock.Verify(m => m.Map<UserDto>(user), Times.Never);

    }
}