using Xunit;
using Todo.Application.Team.Commands.UpdateTaskByTeam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Application.Team.Commands.UpdateTeamTask;
using Todo.Domain.Enums;
using FluentValidation.TestHelper;

namespace Todo.Application.Team.Commands.UpdateTaskByTeam.Tests
{
    public class UpdateTaskByTeamCommandValidatorTests
    {
        [Fact()]
        public void Validator_ForValidUpdateTaskByTeamCommand_ShouldNotHaveValidationErrors()
        {

            // arrange
            var updateTaskByTeamCommand = new UpdateTaskByTeamCommand(new Guid(), new Guid(), "test", "test", (Priority)1);

            var validator = new UpdateTaskByTeamCommandValidator();

            // act

            var result = validator.TestValidate(updateTaskByTeamCommand);

            // assert

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact()]
        public void Validator_ForInvalidUpdateTaskByTeamCommand_ShouldHaveValidationErrors()
        {

            // arrange
            var updateTaskByTeamCommand = new UpdateTaskByTeamCommand(new Guid(), new Guid(), "", "", (Priority)5);

            var validator = new UpdateTaskByTeamCommandValidator();

            // act

            var result = validator.TestValidate(updateTaskByTeamCommand);

            // assert
            result.ShouldHaveValidationErrorFor(e => e.Title).WithErrorMessage("Title is required");
            result.ShouldHaveValidationErrorFor(e => e.Description).WithErrorMessage("Description is required");
            result.ShouldHaveValidationErrorFor(e => e.Priority).WithErrorMessage("Value are not allowed");

        }

        [Theory()]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]

        public void Validator_ForValidUpdateTaskByTeamCommand_ShouldNotHaveValidationErrorForPriority(int priorityValue)
        {

            // arrange
            var updateTaskByTeamCommand = new UpdateTaskByTeamCommand(new Guid(), new Guid(), "", "", (Priority)priorityValue);

            var validator = new UpdateTaskByTeamCommandValidator();

            // act

            var result = validator.TestValidate(updateTaskByTeamCommand);

            // assert
            result.ShouldNotHaveValidationErrorFor(e => e.Priority);

        }


        [Fact()]
        public void Validator_ForInvalidUpdateTaskByTeamCommand_ShouldHaveValidationErrorForPriority()
        {

            // arrange
            var updateTaskByTeamCommand = new UpdateTaskByTeamCommand(new Guid(), new Guid(), "", "", (Priority)5);

            var validator = new UpdateTaskByTeamCommandValidator();

            // act

            var result = validator.TestValidate(updateTaskByTeamCommand);

            // assert
            result.ShouldHaveValidationErrorFor(e => e.Priority).WithErrorMessage("Value are not allowed");

        }
    }
}