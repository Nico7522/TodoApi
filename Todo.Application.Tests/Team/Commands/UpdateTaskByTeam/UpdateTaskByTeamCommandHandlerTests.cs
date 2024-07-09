﻿using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Xunit;
using AsyncTask = System.Threading.Tasks.Task;

namespace Todo.Application.Team.Commands.UpdateTeamTask.Tests;

public class UpdateTaskByTeamCommandHandlerTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<ITodoRepository> _todoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IAuthorization<TeamEntity>> _teamAuthorizationMock;
    private readonly Mock<IValidator<UpdateTaskByTeamCommand>> _validatorMock;
    private readonly UpdateTaskByTeamCommandHandler _handler;
    private readonly Guid _teamId;
    private readonly Guid _taskId;


    public UpdateTaskByTeamCommandHandlerTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _todoRepositoryMock = new Mock<ITodoRepository>();
        _mapperMock = new Mock<IMapper>();
        _teamAuthorizationMock = new Mock<IAuthorization<TeamEntity>>();
        _validatorMock = new Mock<IValidator<UpdateTaskByTeamCommand>>();
        _handler = new UpdateTaskByTeamCommandHandler(_teamRepositoryMock.Object, _todoRepositoryMock.Object, _mapperMock.Object, _teamAuthorizationMock.Object, _validatorMock.Object);
        _teamId = Guid.NewGuid();
        _taskId = Guid.NewGuid();
    }
    [Fact()]
    public async AsyncTask Handle_ForValidCommand_ShouldUpdateTaskCorrectly()
    {
        // arrange
        var task = new TodoEntity()
        {
           Id = _taskId,
           TeamId = _teamId,
           Title = "cc",
           Description = "gg",
           Priority = Domain.Enums.Priority.High
        };

        var team = new TeamEntity() { Id = _teamId, Tasks = new List<TodoEntity> { task } };
        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new UpdateTaskByTeamCommand(_taskId, _teamId, "test", "test", Domain.Enums.Priority.Low);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());


        // act

        await _handler.Handle(command, CancellationToken.None);


        // assert
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _todoRepositoryMock.Verify(r => r.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Once);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map(command, task), Times.Once);
    }

    [Fact()]
    public async AsyncTask Handle_ForNoExistingTeam_ShouldThrowNotFoundException()
    {
        // arrange
        var task = new TodoEntity()
        {
            Id = _taskId,
            TeamId = _teamId,
            Title = "cc",
            Description = "gg",
            Priority = Domain.Enums.Priority.High
        };
        var team = new TeamEntity() { Id = _teamId, Tasks = new List<TodoEntity> { task } };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync((TeamEntity?)null);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new UpdateTaskByTeamCommand(_taskId, _teamId, "test", "test", Domain.Enums.Priority.Low);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());


        // act

        Func<AsyncTask> act = async() => await _handler.Handle(command, CancellationToken.None);


        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Team not found");
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Never);
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _todoRepositoryMock.Verify(r => r.GetById(_taskId), Times.Never);
        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Never);
        _mapperMock.Verify(m => m.Map(command, task), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForNoExistingTask_ShouldThrowNotFoundException()
    {
        // arrange
        var task = new TodoEntity()
        {
            Id = _taskId,
            TeamId = _teamId,
            Title = "cc",
            Description = "gg",
            Priority = Domain.Enums.Priority.High
        };
        var team = new TeamEntity() { Id = _teamId, Tasks = new List<TodoEntity> { task } };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync((TodoEntity?)null);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new UpdateTaskByTeamCommand(_taskId, _teamId, "test", "test", Domain.Enums.Priority.Low);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());


        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);


        // assert
        await act.Should().ThrowAsync<NotFoundException>().WithMessage("Task not found");
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Never);
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _todoRepositoryMock.Verify(r => r.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Never);
        _mapperMock.Verify(m => m.Map(command, task), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForTaskNotInTeam_ShouldThrowBadRequestException()
    {
        // arrange
        var task = new TodoEntity()
        {
            Id = _taskId,
            TeamId = Guid.NewGuid(),
            Title = "cc",
            Description = "gg",
            Priority = Domain.Enums.Priority.High
        };
        var team = new TeamEntity() { Id = _teamId, Tasks = new List<TodoEntity> { task } };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new UpdateTaskByTeamCommand(_taskId, _teamId, "test", "test", Domain.Enums.Priority.Low);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());


        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);


        // assert
        await act.Should().ThrowAsync<BadRequestException>().WithMessage("Task not in team");
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Never);
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _todoRepositoryMock.Verify(r => r.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Never);
        _mapperMock.Verify(m => m.Map(command, task), Times.Never);
    }

    [Fact()]
    public async AsyncTask Handle_ForUnauthoriazedUserAction_ShouldThrowForbidException()
    {
        // arrange
        var task = new TodoEntity()
        {
            Id = _taskId,
            TeamId = _teamId,
            Title = "cc",
            Description = "gg",
            Priority = Domain.Enums.Priority.High
        };
        var team = new TeamEntity() { Id = _teamId, Tasks = new List<TodoEntity> { task } };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(false);
        var command = new UpdateTaskByTeamCommand(_taskId, _teamId, "test", "test", Domain.Enums.Priority.Low);
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());


        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);


        // assert
        await act.Should().ThrowAsync<ForbidException>().WithMessage("Your not authorized");
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _todoRepositoryMock.Verify(r => r.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Never);
        _mapperMock.Verify(m => m.Map(command, task), Times.Never);
    }
    [Fact()]
    public async AsyncTask Handle_ForUnValidCommand_ShouldThrowValidationException()
    {
        // arrange
        var task = new TodoEntity()
        {
            Id = _taskId,
            TeamId = _teamId,
            Title = "cc",
            Description = "gg",
            Priority = Domain.Enums.Priority.High
        };
        var team = new TeamEntity() { Id = _teamId, Tasks = new List<TodoEntity> { task } };

        _teamRepositoryMock.Setup(r => r.GetById(_teamId)).ReturnsAsync(team);
        _todoRepositoryMock.Setup(r => r.GetById(_taskId)).ReturnsAsync(task);
        _teamAuthorizationMock.Setup(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update)).Returns(true);
        var command = new UpdateTaskByTeamCommand(_taskId, _teamId, "test", "test", Domain.Enums.Priority.Low);
    
        _validatorMock.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult(new List<ValidationFailure>() { new ValidationFailure() }));


        // act

        Func<AsyncTask> act = async () => await _handler.Handle(command, CancellationToken.None);


        // assert
        await act.Should().ThrowAsync<ValidationException>();
        _teamAuthorizationMock.Verify(a => a.Authorize(team, Domain.Enums.RessourceOperation.Update), Times.Once);
        _teamRepositoryMock.Verify(r => r.GetById(_teamId), Times.Once);
        _todoRepositoryMock.Verify(r => r.GetById(_taskId), Times.Once);
        _todoRepositoryMock.Verify(r => r.SaveChanges(), Times.Never);
        _validatorMock.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map(command, task), Times.Never);
    }
}