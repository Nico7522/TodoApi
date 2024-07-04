using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Moq;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;

namespace Todo.Application.Task.Commands.UpdateTask.Tests;

public class UpdateTaskCommandHandlerTests
{
    private readonly Mock<ITodoRepository> _todoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateTaskCommandHandler _handler;
    private readonly Mock<IValidator<UpdateTaskCommand>> _validatorMock;


    public UpdateTaskCommandHandlerTests()
    {
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<UpdateTaskCommand>>();
        _handler = new UpdateTaskCommandHandler(_mapperMock.Object, _todoRepositoryMock.Object, _validatorMock.Object);
    }
    [Fact()]
    public async System.Threading.Tasks.Task Handle_WithValidRequest_ShouldUpdateTaskCorrectly()
    {
        // arrange
        var taskId = new Guid();

        var baseTask = new TodoEntity()
        {
            Id = taskId,
            Title = "Title",
            Description = "Description",
            Priority = Domain.Enums.Priority.Low
        };

        var command = new UpdateTaskCommand(taskId, "test", "test", Domain.Enums.Priority.High);

        _todoRepositoryMock.Setup(r => r.GetById(taskId)).ReturnsAsync(baseTask);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // act

        await _handler.Handle(command, CancellationToken.None);

        // assert
        
        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        _mapperMock.Verify(m => m.Map(command, baseTask), Times.Once);
            
    }

    [Fact()]
    public async System.Threading.Tasks.Task Handle_WithNoExistingTask_ShouldThrowNotFoundException()
    {
        // arrange
        var taskId = new Guid();

        var baseTask = new TodoEntity()
        {
            Id = taskId,
            Title = "Title",
            Description = "Description",
            Priority = Domain.Enums.Priority.Low
        };

        var command = new UpdateTaskCommand(new Guid(), "test", "test", Domain.Enums.Priority.High);

        _todoRepositoryMock.Setup(r => r.GetById(taskId)).ReturnsAsync((TodoEntity?)null);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // act

        Func<System.Threading.Tasks.Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // assert

        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Task not found");
        _mapperMock.Verify(m => m.Map(command, baseTask), Times.Never);

    }
}