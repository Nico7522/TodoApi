using AutoMapper;
using FluentAssertions;
using Moq;
using Todo.Application.Team.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;


namespace Todo.Application.Team.Queries.GetTeamById.Tests;

public class GetTeamByIdQueryHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTeamByIdQueryHandler _handler;
    private readonly Guid _teamId;
    public GetTeamByIdQueryHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTeamByIdQueryHandler(_teamRepositoryMock.Object, _mapperMock.Object);
        _teamId = Guid.NewGuid();
    }
    [Fact()]
    public async TaskAsync Handle_ForValidQuery_ShouldReturnTeamCorrectly()
    {
        // arrange

        var team = new TeamEntity() { Id = _teamId, IsActive = true };

        var query = new GetTeamByIdQuery(_teamId);
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);

        // act

        await _handler.Handle(query, CancellationToken.None);

        // assert

        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _mapperMock.Verify(m => m.Map<TeamDto>(team), Times.Once);
    }

    [Fact()]
    public async TaskAsync Handle_ForUnvalidQuery_ShouldThrowNotFoundException()
    {
        // arrange

        var team = new TeamEntity() { Id = _teamId, IsActive = true };

        var query = new GetTeamByIdQuery(_teamId);
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync((TeamEntity?)null);

        // act

        Func<TaskAsync> act = async() => await _handler.Handle(query, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not found");
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _mapperMock.Verify(m => m.Map<TeamDto>(team), Times.Never);
    }
}