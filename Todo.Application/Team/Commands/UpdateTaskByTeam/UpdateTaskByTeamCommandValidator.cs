using FluentValidation;
using Todo.Application.Team.Commands.UpdateTeamTask;

namespace Todo.Application.Team.Commands.UpdateTaskByTeam;

public class UpdateTaskByTeamCommandValidator : AbstractValidator<UpdateTaskByTeamCommand>
{
    public UpdateTaskByTeamCommandValidator()
    {
        RuleFor(form => form.TaskId).NotEmpty().WithMessage("Task id is required");
        RuleFor(form => form.TeamId).NotEmpty().WithMessage("Team id is required");

        RuleFor(form => form.Title).NotEmpty().WithMessage("Title is required").MaximumLength(100);
        RuleFor(form => form.Description).NotEmpty().WithMessage("Description is required").MaximumLength(300);
        RuleFor(form => form.Priority).NotNull().WithMessage("Priority is empty");
        RuleFor(form => form.Priority).IsInEnum().WithMessage("Value are not allowed");
    }
}
