using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Todo.Application.Team.Dto;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Xunit;


namespace Todo.Application.Users.Queries.GetTeamByUser.Tests;

public class GetTeamByUserQueryHandlerTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTeamByUserQueryHandler _handler;
    private readonly string _userId;
    private readonly Guid _teamId;

    public GetTeamByUserQueryHandlerTests()
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
        _handler = new GetTeamByUserQueryHandler(_userManagerMock.Object, _mapperMock.Object );
        _userId = "userId";
        _teamId = Guid.NewGuid();

    }
    [Fact()]
    public async System.Threading.Tasks.Task Handle_ForValidCommand_ShouldGetUserTeamCorrectly()
    {
        var team = new TeamEntity()
        {
            Id = _teamId
        };
        var teamDto = new TeamDto() { Id = team.Id };
        // arrange
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = _teamId,
            Team = team,
            FirstName = "name",
            LastName = "name",
            
        };
        var command = new GetTeamByUserQuery(user.Id);

        var userList = new List<UserEntity>() { user };

        _userManagerMock.Setup(r => r.Users).Returns(userList.AsQueryable());
        _mapperMock.Setup(m => m.Map<TeamDto>(team)).Returns(teamDto);

        // act

        await _handler.Handle(command, CancellationToken.None);

        //assert
        _userManagerMock.Verify(m => m.Users, Times.Once);
        _mapperMock.Verify(m => m.Map<TeamDto>(team), Times.Once);
    }

    [Fact()]
    public async System.Threading.Tasks.Task Handle_ForUserNotFound_ShouldThrowNotFoundException()
    {
        var team = new TeamEntity()
        {
            Id = _teamId
        };
        var teamDto = new TeamDto() { Id = team.Id };
        // arrange
        var user = new UserEntity()
        {
            Id = _userId,
            TeamId = _teamId,
            Team = team,
            FirstName = "name",
            LastName = "name",

        };
        var command = new GetTeamByUserQuery(user.Id);

        var userList = new List<UserEntity>() {  };

        _userManagerMock.Setup(r => r.Users).Returns(userList.AsQueryable());
        _mapperMock.Setup(m => m.Map<TeamDto>(team)).Returns(teamDto);

        // act

         Func<System.Threading.Tasks.Task> act = async() => await _handler.Handle(command, CancellationToken.None);

        //assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("User not found");
        _userManagerMock.Verify(m => m.Users, Times.Once);
        _mapperMock.Verify(m => m.Map<TeamDto>(team), Times.Never);
    }
}