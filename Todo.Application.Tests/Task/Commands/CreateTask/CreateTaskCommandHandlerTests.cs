using AutoMapper;
using FluentAssertions;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Xunit;

namespace Todo.Application.Task.Commands.CreateTask.Tests;

public class CreateTaskCommandHandlerTests
{
    [Fact()]
    public async void Handle_ForValidCommand_ReturnCreatedTaskId()
    {
        // arrange
        var guid = new Guid();
        var mapperMock = new Mock<IMapper>();

        var command = new CreateTaskCommand();
        var taskEntity = new TodoEntity { Id = guid, Title = "Test", Description = "test", Priority = Domain.Enums.Priority.Low };

        mapperMock.Setup(m => m.Map<TodoEntity>(command)).Returns(taskEntity);
        var todoRepositoryMock = new Mock<ITodoRepository>();
        todoRepositoryMock.Setup(repo => repo.Create(It.IsAny<TodoEntity>())).ReturnsAsync(guid);

        var commandHandler = new CreateTaskCommandHandler(mapperMock.Object, todoRepositoryMock.Object);

        // act

       var result = await commandHandler.Handle(command, CancellationToken.None);

        // assert

        result.Should().Be(guid.ToString());
        todoRepositoryMock.Verify(r => r.Create(taskEntity), Times.Once);
    }
}