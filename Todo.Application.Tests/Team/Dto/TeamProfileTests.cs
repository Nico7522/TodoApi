using Xunit;
using AutoMapper;
using Todo.Application.Task.Dto;
using Todo.Application.Users.Dto;
using Todo.Domain.Entities;
using FluentAssertions;

namespace Todo.Application.Team.Dto.Tests;


//public class TeamProfileTests
//{

//    private readonly IMapper _mapper;
//    public TeamProfileTests()
//    {

//        var configuration = new MapperConfiguration(cfg =>
//        {
//            cfg.AddProfile<TeamProfile>();
//            cfg.AddProfile<TodoProfile>();
//            cfg.AddProfile<UserProfile>();

//        });

//        _mapper = configuration.CreateMapper();

//    }

//    [Fact()]
//    public void CreateMap_ForTeamEntityToTeamDto_MapsCorrectly()
//    {

//        // arrange

//        var leader = new UserEntity()
//        {
//            Id = "qdqdsqd",
//            UserName = "test@gmail.com",
//            Email = "test@gmail.com",
//            PhoneNumber = "491414141",
//            FirstName=  "test",
//            LastName = "test",
//            Birthdate = new DateOnly(2000,01,01),
//            HireDate = new DateOnly(2020,01,01)
//        };

//        var simpleUser = new UserEntity()
//        {
//            Id = "qdqsdqdsqsqsqsq",
//            UserName = "test@gmail.com",
//            Email = "test2@gmail.com",
//            PhoneNumber = "491414141",
//            FirstName = "test",
//            LastName = "test",
//            Birthdate = new DateOnly(2000, 01, 01),
//            HireDate = new DateOnly(2020, 01, 01)
//        };


//        ICollection<UserEntity> userList = new List<UserEntity>();
//        userList.Add(leader);
//        userList.Add(simpleUser);

//        var taskEntity = new TodoEntity()
//        {
//            Title = "Test",
//            Description = "Test",
//            Priority = Domain.Enums.Priority.Low,
//            UserId = "dsqd-qdsqsd44-qsd",
//            User = new UserEntity()
//            {
//                Id = "dsqd-qdsqsd44-qsd",
//                FirstName = "Test",
//                LastName = "Test",
//                Birthdate = new DateOnly(2000, 01, 01),
//                HireDate = new DateOnly(2010, 01, 01),
//                Email = "test@gmail.com",
//            }
//        };
//        var taskEntity2 = new TodoEntity()
//        {
//            Title = "Test",
//            Description = "Test",
//            Priority = Domain.Enums.Priority.Low,
//            UserId = "dsqd-qdsqsd44-qsd",
//            User = new UserEntity()
//            {
//                Id = "dsqd-qdsqsd44-qsd",
//                FirstName = "Test",
//                LastName = "Test",
//                Birthdate = new DateOnly(2000, 01, 01),
//                HireDate = new DateOnly(2010, 01, 01),
//                Email = "test@gmail.com",
//            }
//        };

//        ICollection<TodoEntity> todoList = new List<TodoEntity>();
//        todoList.Add(taskEntity); 
//        todoList.Add(taskEntity2);

//        //var teamEntity = new TeamEntity() { IsActive = true, Id = new Guid(), Leader = leader, LeaderId = "qdqdsqd",  Name = "dqsd", Tasks = todoList, Users = userList };

//        // act

//        //var teamDto = _mapper.Map<TeamDto>(teamEntity);

//        // assert

//        teamDto.Should().NotBeNull();
//        teamDto.Tasks.Should().HaveCount(2);
//        teamDto.Users.Should().HaveCount(2);

//        var taskDto1 = teamDto.Tasks.First();
//        taskDto1.Title.Should().Be(taskEntity.Title);
//        taskDto1.Description.Should().Be(taskEntity.Description);
//        taskDto1.Priority.Should().Be(taskEntity.Priority);
//        taskDto1.User.Should().NotBeNull();
//        taskDto1.User.Id.Should().Be(taskEntity.User.Id);
//        taskDto1.User.FirstName.Should().Be(taskEntity.User.FirstName);
//        taskDto1.User.LastName.Should().Be(taskEntity.User.LastName);
//        taskDto1.User.Birthdate.Should().Be(taskEntity.User.Birthdate);
//        taskDto1.User.HireDate.Should().Be(taskEntity.User.HireDate);
//        taskDto1.User.Email.Should().Be(taskEntity.User.Email);

//        var taskDto2 = teamDto.Tasks.Last();
//        taskDto2.Title.Should().Be(taskEntity2.Title);
//        taskDto2.Description.Should().Be(taskEntity2.Description);
//        taskDto2.Priority.Should().Be(taskEntity2.Priority);
//        taskDto2.User.Should().NotBeNull();
//        taskDto2.User.Id.Should().Be(taskEntity2.User.Id);
//        taskDto2.User.FirstName.Should().Be(taskEntity2.User.FirstName);
//        taskDto2.User.LastName.Should().Be(taskEntity2.User.LastName);
//        taskDto2.User.Birthdate.Should().Be(taskEntity2.User.Birthdate);
//        taskDto2.User.HireDate.Should().Be(taskEntity2.User.HireDate);
//        taskDto2.User.Email.Should().Be(taskEntity2.User.Email);

//        var user = teamDto.Users.First();
//        user.Id.Should().Be(leader.Id);
//        user.Email.Should().Be(leader.Email);
//        user.FirstName.Should().Be(leader.FirstName);
//        user.LastName.Should().Be(leader.LastName);
//        user.HireDate.Should().Be(leader.HireDate);
//        user.Birthdate.Should().Be(leader.Birthdate);

//        var user2 = teamDto.Users.Last();
//        user2.Id.Should().Be(simpleUser.Id);
//        user2.Email.Should().Be(simpleUser.Email);
//        user2.FirstName.Should().Be(simpleUser.FirstName);
//        user2.LastName.Should().Be(simpleUser.LastName);
//        user2.HireDate.Should().Be(simpleUser.HireDate);
//        user2.Birthdate.Should().Be(simpleUser.Birthdate);



//    }
//}