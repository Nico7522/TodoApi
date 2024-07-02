﻿using Xunit;
using Todo.Application.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Todo.Domain.Entities;
using FluentAssertions;
using Todo.Application.Task.Dto;

namespace Todo.Application.Users.Dto.Tests
{
    public class UserProfileTests
    {
        private readonly IMapper _mapper;
        public UserProfileTests()
        {

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<TaskProfile>();

            });

            _mapper = configuration.CreateMapper();

        }
        [Fact()]
        public void CreateMap_ForUserEntityToUserDtoWithTeamId_MapsCorrectly()
        {
            // arrange

            var task = new TodoEntity()
            {
                Id = new Guid(),
                Title = "Title",
                Description = "Description",
                Priority = Domain.Enums.Priority.Low,
                ClosingDate = new DateOnly(2050,01,01),
                CreationDate = new DateOnly(2045, 01, 01),
                IsComplete = true,
            };

            var task2 = new TodoEntity()
            {
                Id = new Guid(),
                Title = "Title2",
                Description = "Description2",
                Priority = Domain.Enums.Priority.High,
                CreationDate = new DateOnly(2044, 01, 01),
                IsComplete = false,
                


            };

            var tasks = new List<TodoEntity>() { task, task2 };

            var userEntity = new UserEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "test@gmail.com",
                Email = "test@gmail.com",
                PasswordHash = Guid.NewGuid().ToString(),
                Birthdate = new DateOnly(2020, 01, 01),
                HireDate = new DateOnly(2010, 01, 01),
                PhoneNumber = "419424242",
                TeamId = new Guid(),
                Tasks = tasks
            };

            // act

            var userDto = _mapper.Map<UserDto>(userEntity);

            // assert

            var taskDto1 = userDto.Tasks.First();
            var taskDto2 = userDto.Tasks.Last();

            userDto.Should().NotBeNull();
            userDto.Tasks.Should().HaveCount(2);
            userDto.Id.Should().Be(userEntity.Id);
            userDto.Email.Should().Be(userEntity.Email);
            userDto.Birthdate.Should().Be(userEntity.Birthdate);
            userDto.HireDate.Should().Be(userEntity.HireDate);
            userDto.LastName.Should().Be(userEntity.LastName);
            userDto.FirstName.Should().Be(userEntity.FirstName);
            userDto.PhoneNumber.Should().Be(userEntity.PhoneNumber);
            userDto.TeamId.Should().Be((Guid)userEntity.TeamId);

            taskDto1.Id.Should().Be(task.Id);
            taskDto1.Title.Should().Be(task.Title);
            taskDto1.Description.Should().Be(task.Description);
            taskDto1.Priority.Should().Be(task.Priority);
            taskDto1.IsComplete.Should().Be(task.IsComplete);
            taskDto1.CreationDate.Should().Be(task.CreationDate);
            taskDto1.ClosingDate.Should().Be(task.ClosingDate);



            taskDto2.Id.Should().Be(task2.Id);
            taskDto2.Title.Should().Be(task2.Title);
            taskDto2.Description.Should().Be(task2.Description);
            taskDto2.Priority.Should().Be(task2.Priority);
            taskDto2.IsComplete.Should().Be(task2.IsComplete);
            taskDto2.CreationDate.Should().Be(task2.CreationDate);
            taskDto2.ClosingDate.Should().BeNull();

        }

        [Fact()]
        public void CreateMap_ForUserEntityToUserDtoWithTeamIdNull_MapsCorrectly()
        {
            // arrange

            var task = new TodoEntity()
            {
                Id = new Guid(),
                Title = "Title",
                Description = "Description",
                Priority = Domain.Enums.Priority.Low,
                ClosingDate = new DateOnly(2050, 01, 01),
                CreationDate = new DateOnly(2045, 01, 01),
                IsComplete = true,
            };

            var task2 = new TodoEntity()
            {
                Id = new Guid(),
                Title = "Title2",
                Description = "Description2",
                Priority = Domain.Enums.Priority.High,
                CreationDate = new DateOnly(2044, 01, 01),
                IsComplete = false,



            };

            var tasks = new List<TodoEntity>() { task, task2 };

            var userEntity = new UserEntity()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "test@gmail.com",
                Email = "test@gmail.com",
                PasswordHash = Guid.NewGuid().ToString(),
                Birthdate = new DateOnly(2020, 01, 01),
                HireDate = new DateOnly(2010, 01, 01),
                PhoneNumber = "419424242",
                Tasks = tasks,
                TeamId = null
            };

            // act

            var userDto = _mapper.Map<UserDto>(userEntity);

            // assert

            var taskDto1 = userDto.Tasks.First();
            var taskDto2 = userDto.Tasks.Last();

            userDto.Should().NotBeNull();
            userDto.Tasks.Should().HaveCount(2);
            userDto.Id.Should().Be(userEntity.Id);
            userDto.Email.Should().Be(userEntity.Email);
            userDto.Birthdate.Should().Be(userEntity.Birthdate);
            userDto.HireDate.Should().Be(userEntity.HireDate);
            userDto.LastName.Should().Be(userEntity.LastName);
            userDto.FirstName.Should().Be(userEntity.FirstName);
            userDto.PhoneNumber.Should().Be(userEntity.PhoneNumber);
            userDto.TeamId.Should().BeEmpty();

            taskDto1.Id.Should().Be(task.Id);
            taskDto1.Title.Should().Be(task.Title);
            taskDto1.Description.Should().Be(task.Description);
            taskDto1.Priority.Should().Be(task.Priority);
            taskDto1.IsComplete.Should().Be(task.IsComplete);
            taskDto1.CreationDate.Should().Be(task.CreationDate);
            taskDto1.ClosingDate.Should().Be(task.ClosingDate);



            taskDto2.Id.Should().Be(task2.Id);
            taskDto2.Title.Should().Be(task2.Title);
            taskDto2.Description.Should().Be(task2.Description);
            taskDto2.Priority.Should().Be(task2.Priority);
            taskDto2.IsComplete.Should().Be(task2.IsComplete);
            taskDto2.CreationDate.Should().Be(task2.CreationDate);
            taskDto2.ClosingDate.Should().BeNull();

        }
    }
}