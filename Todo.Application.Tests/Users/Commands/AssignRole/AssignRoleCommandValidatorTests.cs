using Xunit;
using Todo.Application.Users.Commands.AssignRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;

namespace Todo.Application.Users.Commands.AssignRole.Tests
{
    public class AssignRoleCommandValidatorTests
    {
        [Theory()]
        [InlineData("SuperAdmin")]
        [InlineData("Admin")]
        [InlineData("Leader")]
        [InlineData("User")]

        public void Validator_ForValidAssignRoleCommand_ShouldNotHaveValidationErrors(string role)
        {
            // arrange

            var assignRoleCommand = new AssignRoleCommand("48448", role);
            var validator = new AssignRoleCommandValidator();

            // act

            var result = validator.TestValidate(assignRoleCommand);

            // assert

            result.ShouldNotHaveAnyValidationErrors();  
        }

        [Theory()]
        [InlineData("test")]
        [InlineData("")]
        public void Validator_ForInvalidAssignRoleCommand_ShouldHaveValidationErrorForRole(string role)
        {
            // arrange

            var assignRoleCommand = new AssignRoleCommand("48448", role);
            var validator = new AssignRoleCommandValidator();

            // act

            var result = validator.TestValidate(assignRoleCommand);

            // assert

            result.ShouldHaveValidationErrorFor(e => e.Role);
        }

        [Fact()]
        public void Validator_ForInvalidAssignRoleCommand_ShouldHaveValidationErrorForId()
        {
            // arrange

            var assignRoleCommand = new AssignRoleCommand("", "SuperAdmin");
            var validator = new AssignRoleCommandValidator();

            // act

            var result = validator.TestValidate(assignRoleCommand);

            // assert

            result.ShouldHaveValidationErrorFor(e => e.UserId).WithErrorMessage("Required field");
        }
    }
}