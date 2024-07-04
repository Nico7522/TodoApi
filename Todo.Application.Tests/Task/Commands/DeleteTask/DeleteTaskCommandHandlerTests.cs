using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Xunit;


namespace Todo.Application.Task.Commands.DeleteTask.Tests;

public class DeleteTaskCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepositoryMock;
    private readonly DeleteTaskCommandHandler _handler;

    public DeleteTaskCommandHandlerTests()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _handler = new DeleteTaskCommandHandler(_todoRepositoryMock.Object);
    }
    [Fact()]
    public async System.Threading.Tasks.Task Handle_WithValidRequest_ShouldDeleteTask()
    {
        // arrange

        var taskToDelete = new TodoEntity() { Id = Guid.NewGuid(), Title = "test", Description = "test" };
         _todoRepositoryMock.Setup(r => r.GetById(taskToDelete.Id)).ReturnsAsync(taskToDelete);
        var command = new DeleteTaskCommand(taskToDelete.Id);
        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert

        _todoRepositoryMock.Verify(r => r.Delete(taskToDelete), Times.Once());

    }
}