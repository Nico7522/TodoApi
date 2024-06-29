using Xunit;
using Todo.Application.Users.Commands.ResetPasswordConfirm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;

namespace Todo.Application.Users.Commands.ResetPasswordConfirm.Tests
{
    public class ResetPasswordConfirmCommandValidatorTests
    {
        [Fact()]
        public void Validator_ForValidResetPasswordConfirmCommand_ShouldNotHaveValidationErrors()
        {

            // arrange

            var ResetPasswordConfirmCommand = new ResetPasswordConfirmCommand("dqdqsdqsdqddsfs", "dsdqsdsqdsd", "@Test12345", "@Test12345");

            var validator = new ResetPasswordConfirmCommandValidator();

            // act

            var result = validator.TestValidate(ResetPasswordConfirmCommand);

            // assert

            result.ShouldNotHaveAnyValidationErrors();

        }

        [Fact()]
        public void Validator_ForInvalidResetPasswordConfirmCommand_ShouldHaveValidationErrorForPassword()
        {

            // arrange

            var ResetPasswordConfirmCommand = new ResetPasswordConfirmCommand("dqdqsdqsdqddsfs", "dsdqsdsqdsd", "@Test12345", "@Test1234");

            var validator = new ResetPasswordConfirmCommandValidator();

            // act

            var result = validator.TestValidate(ResetPasswordConfirmCommand);

            // assert

            result.ShouldHaveValidationErrorFor(e => e.Password).WithErrorMessage("Password not match");

        }
    }
}