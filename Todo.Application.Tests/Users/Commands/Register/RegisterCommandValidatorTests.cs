using Xunit;
using FluentValidation.TestHelper;

namespace Todo.Application.Users.Commands.Register.Tests;

public class RegisterCommandValidatorTests
{
    [Fact()]
    public void Validator_ForValidRegisterCommand_ShouldNotHaveValidationErrors()
    {
        // arrange
        var registerCommand = new RegisterCommand()
        {
            FirstName = "Jean",
            LastName = "Pierre",
            BirthDate = new DateOnly(2021, 2, 1),
            Password = "@Password12345",
            PasswordConfirm = "@Password12345",
            HireDate = new DateOnly(2021, 2, 1),
            Email = "test@gmail.com",
            PhoneNumber = "491419252"
        };

        var validator = new RegisterCommandValidator();

        // act

        var result = validator.TestValidate(registerCommand);

        // assert

        result.ShouldNotHaveAnyValidationErrors();

    }

    [Fact()]
    public void Validator_ForInvalidRegisterCommand_ShouldHaveValidationErrorForPassword()
    {
        // arrange
        var registerCommand = new RegisterCommand()
        {
            FirstName = "Jean",
            LastName = "Pierre",
            BirthDate = new DateOnly(2021, 2, 1),
            Password = "@Password12345",
            PasswordConfirm = "@Password1234",
            HireDate = new DateOnly(2021, 2, 1),
            Email = "test@gmail.com",
            PhoneNumber = "491419252"
        };

        var validator = new RegisterCommandValidator();

        // act

        var result = validator.TestValidate(registerCommand);

        // assert

        result.ShouldHaveValidationErrorFor(e => e.Password).WithErrorMessage("Password not match");

    }

    [Fact()]
    public void Validator_ForInvalidRegisterCommand_ShouldHaveValidationErrorForPhoneNumber()
    {
        // arrange
        var registerCommand = new RegisterCommand()
        {
            FirstName = "Jean",
            LastName = "Pierre",
            BirthDate = new DateOnly(2021, 2, 1),
            Password = "@Password12345",
            PasswordConfirm = "@Password12345",
            HireDate = new DateOnly(2021, 2, 1),
            Email = "test@gmail.com",
            PhoneNumber = "49141925"
        };

        var validator = new RegisterCommandValidator();

        // act

        var result = validator.TestValidate(registerCommand);

        // assert

        result.ShouldHaveValidationErrorFor(e => e.PhoneNumber).WithErrorMessage("Invalid phone number");

    }

    [Fact()]
    public void Validator_ForInvalidRegisterCommand_ShouldHaveValidationErrorForEmail()
    {
        // arrange
        var registerCommand = new RegisterCommand()
        {
            FirstName = "Jean",
            LastName = "Pierre",
            BirthDate = new DateOnly(2021, 2, 1),
            Password = "@Password12345",
            PasswordConfirm = "@Password12345",
            HireDate = new DateOnly(2021, 2, 1),
            Email = "test@gmailcom",
            PhoneNumber = "491419255"
        };

        var validator = new RegisterCommandValidator();

        // act

        var result = validator.TestValidate(registerCommand);

        // assert

        result.ShouldHaveValidationErrorFor(e => e.Email).WithErrorMessage("Email is not valid");

    }

    [Fact()]
    public void Validator_ForInvalidRegisterCommand_ShouldHaveValidationErrorHireDate()
    {
        // arrange
        var registerCommand = new RegisterCommand()
        {
            FirstName = "Jean",
            LastName = "Pierre",
            BirthDate = new DateOnly(2000, 2, 1),
            Password = "@Password12345",
            PasswordConfirm = "@Password12345",
            HireDate = new DateOnly(1999, 2, 1),
            Email = "test@gmail.com",
            PhoneNumber = "491419255"
        };

        var validator = new RegisterCommandValidator();

        // act

        var result = validator.TestValidate(registerCommand);

        // assert

        result.ShouldHaveValidationErrorFor(e => new {e.HireDate, e.BirthDate}).WithErrorMessage("Wrong hire date");

    }
}