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
            cfg.AddProfile<TodoProfile>();
            cfg.AddProfile<UserProfile>();
        });

        _mapper = configuration.CreateMapper();

    }
    [Fact()]
    public void CreateMap_ForTodoEntityToTodoDto_MapsCorrectly()
    {
        // arrange

        var taskEntity = new TodoEntity()
        {
            Title = "Test",
            Description = "Test",
            Priority = Domain.Enums.Priority.Low,
            UserId = "dsqd-qdsqsd44-qsd",
            User = new UserEntity()
            {
                Id = "dsqd-qdsqsd44-qsd",
                FirstName = "Test",
                LastName = "Test",
                Birthdate = new DateOnly(2000,01,01),
                HireDate = new DateOnly(2010,01,01),
                Email = "test@gmail.com",
            }
        };

        // act

        var taskDto = _mapper.Map<TodoDto>(taskEntity);

        // assert

        taskDto.Should().NotBeNull();
        taskDto.Title.Should().Be(taskEntity.Title);
        taskDto.Description.Should().Be(taskEntity.Description);
        taskDto.Priority.Should().Be(taskEntity.Priority);
        taskDto.Id.Should().Be(taskEntity.Id);
        taskDto.User!.Id.Should().Be(taskEntity.User.Id);
        taskDto.User!.Email.Should().Be(taskEntity.User.Email);
        taskDto.User.FirstName.Should().Be(taskEntity.User.FirstName);
        taskDto.User.LastName.Should().Be(taskEntity.User.LastName);
        taskDto.User.Birthdate.Should().Be(taskEntity.User.Birthdate);
        taskDto.User.HireDate.Should().Be(taskEntity.User.HireDate);
    }

    [Fact()]
    public void CreateMap_ForTodoEntityToTodoDto_MapsCorrectlyWithUserNull()
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

        var taskDto = _mapper.Map<TodoDto>(taskEntity);

        // assert

        taskDto.Should().NotBeNull();
        taskDto.Title.Should().Be(taskEntity.Title);
        taskDto.Description.Should().Be(taskEntity.Description);
        taskDto.Priority.Should().Be(taskEntity.Priority);
        taskDto.Id.Should().Be(taskEntity.Id);
        taskDto.User.Should().BeNull();
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