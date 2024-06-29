using Xunit;
using Todo.Application.Users.Commands.ResetPassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;

namespace Todo.Application.Users.Commands.ResetPassword.Tests
{
    public class ResetPasswordCommandValidatorTests
    {
        [Fact()]
        public void Validator_ForValidResetPasswordCommand_ShouldNotHaveValidationErrors()
        {
            // arrange
            var resetPasswordCommand = new ResetPasswordCommand("test@gmail.com");
            var validator = new ResetPasswordCommandValidator();

            // act

            var result = validator.TestValidate(resetPasswordCommand);

            // assert

            result.ShouldNotHaveAnyValidationErrors();
        }
        [Fact()]
        public void Validator_ForInvalidResetPasswordCommand_ShouldHaveValidationErrorEmail()
        {
            // arrange
            var resetPasswordCommand = new ResetPasswordCommand("");
            var validator = new ResetPasswordCommandValidator();

            // act

            var result = validator.TestValidate(resetPasswordCommand);

            // assert

            result.ShouldHaveValidationErrorFor(e => e.Email).WithErrorMessage("Email is required");
        }

        [Theory()]
        [InlineData("testgmail.com")]
        [InlineData("test@gmailcom")]
        public void Validator_ForInvalidResetPasswordCommand_ShouldHaveValidationErrorForEmail(string email)
        {
            // arrange
            var resetPasswordCommand = new ResetPasswordCommand(email);
            var validator = new ResetPasswordCommandValidator();

            // act

            var result = validator.TestValidate(resetPasswordCommand);

            // assert

            result.ShouldHaveValidationErrorFor(e => e.Email).WithErrorMessage("Email is not valid");
        }
    }
}