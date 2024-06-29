using Xunit;
using Todo.Application.Users.Commands.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;

namespace Todo.Application.Users.Commands.Login.Tests
{
    public class LoginCommandValidatorTests
    {
        [Fact()]
        public void Validator_ForValidLoginCommand_ShouldNotHaveValidationErrors()
        {
            // arrange

            var loginCommand = new LoginCommand() { Email = "test@gmail.com", Password = "@Test12345"};
            var validator = new LoginCommandValidator();

            // act

            var result = validator.TestValidate(loginCommand);

            // assert

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory()]
        [InlineData("@Test12345")]
        [InlineData("!Test12345")]
        [InlineData("=Test12345")]
        [InlineData("#Test12345")]
        public void Validator_ForValidLoginCommand_ShouldNotHaveValidationErrorForPassword(string password)
        {
            // arrange

            var loginCommand = new LoginCommand() { Email = "test@gmail.com", Password = password };
            var validator = new LoginCommandValidator();

            // act

            var result = validator.TestValidate(loginCommand);

            // assert

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory()]
        [InlineData("testgmail.com")]
        [InlineData("test@gmailcom")]
        public void Validator_ForInvalidLoginCommand_ShouldHaveValidationErrorForEmail(string email)
        {
            // arrange

            var loginCommand = new LoginCommand() { Email = email, Password = "@Test12345" };
            var validator = new LoginCommandValidator();

            // act

            var result = validator.TestValidate(loginCommand);

            // assert

            result.ShouldHaveValidationErrorFor(e => e.Email).WithErrorMessage("Email is not valid");
        }

        [Theory()]
        [InlineData("@test12345")]
        [InlineData("Test12345")]
        [InlineData("@Tt1")]
        public void Validator_ForInvalidLoginCommand_ShouldHaveValidationErrorForPassword(string password)
        {
            // arrange

            var loginCommand = new LoginCommand() { Email = "test@gmail.com", Password = password };
            var validator = new LoginCommandValidator();

            // act

            var result = validator.TestValidate(loginCommand);

            // assert

            result.ShouldHaveValidationErrorFor(e => e.Password).WithErrorMessage("Password not match the requirement");
        }
    }
}
