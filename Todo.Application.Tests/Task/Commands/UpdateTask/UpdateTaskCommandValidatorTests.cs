using Xunit;
using Todo.Application.Task.Commands.UpdateTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Domain.Enums;
using FluentValidation.TestHelper;

namespace Todo.Application.Task.Commands.UpdateTask.Tests
{
    public class UpdateTaskCommandValidatorTests
    {
        [Fact()]
        public void Validator_ForValidUpdateTaskCommand_ShouldNotHaveValidatorErrors()
        {
            // arrange

            var task = new UpdateTaskCommand(new Guid(), "dqsdqsd", "dqdqsd", (Priority)2);

            var validator = new UpdateTaskCommandValidator();


            // act

            var result = validator.TestValidate(task);

            // assert

            result.ShouldNotHaveAnyValidationErrors();

        }

        [Fact()]
        public void Validator_ForInvalidUpdateTaskCommand_ShouldHaveValidatorErrors()
        {
            // arrange

            var task = new UpdateTaskCommand(new Guid(), "", "", (Priority)5);

            var validator = new UpdateTaskCommandValidator();


            // act

            var result = validator.TestValidate(task);

            // assert

            result.ShouldHaveValidationErrorFor(e => e.Title);
            result.ShouldHaveValidationErrorFor(e => e.Description);
            result.ShouldHaveValidationErrorFor(e => e.Priority);


        }
    }
}