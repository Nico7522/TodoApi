using AutoMapper;
using FluentAssertions;
using Todo.Application.Task.Commands.CreateTask;
using Todo.Application.Task.Commands.UpdateTask;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;
using Xunit;

namespace Todo.Application.Task.Dto.Tests;

public class TodoMapperTests
{
    private readonly IMapper _mapper;
    public TodoMapperTests()
    {

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<TaskProfile>();
            cfg.AddProfile<UserProfile>();
        });

        _mapper = configuration.CreateMapper();

    }
    [Fact()]
    public void CreateMap_ForTodoEntityToTodoDtoWithUserId_MapsCorrectly()
    {
        // arrange

        var taskEntity = new TodoEntity()
        {
            Title = "Test",
            Description = "Test",
            Priority = Domain.Enums.Priority.Low,
            UserId = "dsqd-qdsqsd44-qsd",
        };

        // act

        var taskDto = _mapper.Map<TaskDto>(taskEntity);

        // assert

        taskDto.Should().NotBeNull();
        taskDto.Title.Should().Be(taskEntity.Title);
        taskDto.Description.Should().Be(taskEntity.Description);
        taskDto.Priority.Should().Be(taskEntity.Priority);
        taskDto.Id.Should().Be(taskEntity.Id);
        taskDto.UserId.Should().Be(taskEntity.UserId);
    }

    [Fact()]
    public void CreateMap_ForTodoEntityToTodoDtoWithTeamId_MapsCorrectly()
    {
        // arrange

        var taskEntity = new TodoEntity()
        {
            Title = "Test",
            Description = "Test",
            Priority = Domain.Enums.Priority.Low,
            TeamId = new Guid(),
        };

        // act

        var taskDto = _mapper.Map<TaskDto>(taskEntity);

        // assert

        taskDto.Should().NotBeNull();
        taskDto.Title.Should().Be(taskEntity.Title);
        taskDto.Description.Should().Be(taskEntity.Description);
        taskDto.Priority.Should().Be(taskEntity.Priority);
        taskDto.Id.Should().Be(taskEntity.Id);
        taskDto.TeamId.Should().Be((Guid)taskEntity.TeamId);
        taskDto.UserId.Should().BeNull();
    }

    [Fact()]
    public void CreateMap_ForCreateTaskCommandToTodoEntity_MapsCorrectly()
    {
        // arrange

        var createTaskCommand = new CreateTaskCommand()
        {
            Title = "Test",
            Description = "Test",
            Priority = Domain.Enums.Priority.Low,
        };

        // act

        var taskEntity = _mapper.Map<TodoEntity>(createTaskCommand);

        // assert

        taskEntity.Should().NotBeNull();
        taskEntity.Title.Should().Be(createTaskCommand.Title);
        taskEntity.Description.Should().Be(createTaskCommand.Description);
        taskEntity.Priority.Should().Be(createTaskCommand.Priority);
    }


    [Fact()]
    public void CreateMap_ForUpdateTaskCommandToTodoEntity_MapsCorrectly()
    {
        // arrange

        var updateTaskCommand = new UpdateTaskCommand(new Guid(), "title", "description", Domain.Enums.Priority.Low);

        // act

        var taskEntity = _mapper.Map<TodoEntity>(updateTaskCommand);

        // assert

        taskEntity.Should().NotBeNull();
        taskEntity.Id.Should().Be(updateTaskCommand.Id);
        taskEntity.Title.Should().Be(updateTaskCommand.Title);
        taskEntity.Description.Should().Be(updateTaskCommand.Description);
        taskEntity.Priority.Should().Be(updateTaskCommand.Priority);
    }
}