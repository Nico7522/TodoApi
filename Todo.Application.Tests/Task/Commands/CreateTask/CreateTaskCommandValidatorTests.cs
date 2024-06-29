using FluentValidation.TestHelper;
using Todo.Domain.Enums;
using Xunit;


namespace Todo.Application.Task.Commands.CreateTask.Tests;

public class CreateTaskCommandValidatorTests
{
    [Fact()]
    public void Validator_ForValideCreateTaskCommand_ShouldNotHaveValidationErrors()
    {
        // arrange 

        var task = new CreateTaskCommand() { Title = "Title", Description = "Description", Priority = Domain.Enums.Priority.Medium };

        var validator = new CreateTaskCommandValidator();

        // act

        var result = validator.TestValidate(task);   

        // assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact()]
    public void Validator_ForInvalidCreateTaskCommand_ShouldHaveValidationErrors()
    {
        // arrange 

        var task = new CreateTaskCommand() { Title = "", Description = "", Priority = (Priority)11 };

        var validator = new CreateTaskCommandValidator();

        // act

        var result = validator.TestValidate(task);

        // assert
        result.ShouldHaveValidationErrorFor(c => c.Title);
        result.ShouldHaveValidationErrorFor(c => c.Description);
        result.ShouldHaveValidationErrorFor(c => c.Priority);
    }

    [Theory()]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Validator_ForValidCreateTaskCommand_ShouldNotHaveValidationErrorForPriority(int priority)
    {
        // arrange 

        var task = new CreateTaskCommand() { Title = "", Description = "", Priority = (Priority)priority };

        var validator = new CreateTaskCommandValidator();

        // act

        var result = validator.TestValidate(task);

        // assert
        result.ShouldNotHaveValidationErrorFor(c => c.Priority);
    }

    [Fact()]
    
    public void Validator_ForInvalidCreateTaskCommand_ShouldHaveValidationErrorForPriority()
    {
        // arrange 

        var task = new CreateTaskCommand() { Title = "", Description = "", Priority = (Priority)4 };

        var validator = new CreateTaskCommandValidator();

        // act

        var result = validator.TestValidate(task);

        // assert
        result.ShouldHaveValidationErrorFor(c => c.Priority);
    }
}