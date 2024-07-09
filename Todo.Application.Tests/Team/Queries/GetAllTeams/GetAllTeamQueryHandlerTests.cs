using AutoMapper;
using Moq;
using Todo.Application.Team.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;

namespace Todo.Application.Team.Queries.GetAllTeams.Tests;

public class GetAllTeamQueryHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTeamQueryHandler _handler;

    public GetAllTeamQueryHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllTeamQueryHandler(_teamRepositoryMock.Object, _mapperMock.Object);
    }
    [Fact()]
    public async TaskAsync Handle_ForValidQuery_ShouldReturnTeamsCorrectly()
    {
        // arrange

        var team1 = new TeamEntity() { IsActive = true };
        var team2 = new TeamEntity() { IsActive = true };

        ICollection<TeamEntity> teams = new[] { team1, team2 };
        var query = new GetAllTeamQuery(true);
        _teamRepositoryMock.Setup(r => r.GetAll(query.IsActive)).ReturnsAsync(teams);

        // act

        await _handler.Handle(query, CancellationToken.None);

        // assert

        _teamRepositoryMock.Verify(r => r.GetAll(query.IsActive), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<TeamDto>>(teams), Times.Once);
    }

}