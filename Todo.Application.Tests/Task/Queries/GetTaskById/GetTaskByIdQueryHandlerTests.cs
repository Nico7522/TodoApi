using AutoMapper;
using FluentAssertions;
using Moq;
using Todo.Application.Task.Dto;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using TaskAsync = System.Threading.Tasks.Task;

namespace Todo.Application.Task.Queries.GetTaskById.Tests;

public class GetTaskByIdQueryHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTaskByIdQueryHandler _handler;
    public GetTaskByIdQueryHandlerTests()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetTaskByIdQueryHandler(_todoRepositoryMock.Object, _mapperMock.Object);
    }
    [Fact()]
    public async TaskAsync Handle_ForRetrieveExistingTask_ShouldReturnTaskCorrectly()
    {
        // arrange

        var taskId = Guid.NewGuid();
        var task = new TodoEntity()
        {
            Id = taskId,
            Title = "test",
            Description = "test",
        };

        _todoRepositoryMock.Setup(r => r.GetById(task.Id)).ReturnsAsync(task);
        var query = new GetTaskByIdQuery(taskId);

        // act

        await _handler.Handle(query, CancellationToken.None);

        // assert

        _todoRepositoryMock.Verify(r => r.GetById(task.Id), Times.Once());
        _mapperMock.Verify(m => m.Map<TaskDto>(task), Times.Once);
    }

    [Fact()]
    public async TaskAsync Handle_ForRetrieveNoExistingTask_ShouldThrowNotFoundException()
    {
        // arrange

        var taskId = Guid.NewGuid();
        var task = new TodoEntity()
        {
            Id = Guid.NewGuid(),
            Title = "test",
            Description = "test",
        };

        _todoRepositoryMock.Setup(r => r.GetById(task.Id)).ReturnsAsync((TodoEntity?)null);
        var query = new GetTaskByIdQuery(taskId);

        // act

        Func<TaskAsync> act = async () => await _handler.Handle(query, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Task not found");
    }
}