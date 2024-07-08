using FluentValidation.TestHelper;
using Xunit;


namespace Todo.Application.Team.Commands.CreateTeam.Tests;

public class CreateTeamCommandValidatorTests
{
    [Fact()]
    public void Validator_ForValidCreateTeamCommand_ShouldNotHaveValidationErrors()
    {
        // arrange

        var createTeamCommand = new CreateTeamCommand("test");
        var validator = new CreateTeamCommandValidator();

        // act

        var result = validator.TestValidate(createTeamCommand);

        // assert

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact()]
    public void Validator_ForInvalidCreateTeamCommand_ShouldHaveValidationErrorForEmptyName()
    {
        // arrange

        var createTeamCommand = new CreateTeamCommand("");
        var validator = new CreateTeamCommandValidator();

        // act

        var result = validator.TestValidate(createTeamCommand);

        // assert

        result.ShouldHaveValidationErrorFor(t => t.Name).WithErrorMessage("Name is required");
    }
}