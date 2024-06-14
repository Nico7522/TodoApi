using FluentValidation;

namespace Todo.Api.Forms.UpdateTaskForm;

public class UpdateTaskFormValidator : AbstractValidator<UpdateTaskForm>
{
    public UpdateTaskFormValidator()
    {
        RuleFor(form => form.Title).NotEmpty().MaximumLength(100);
        RuleFor(form => form.Description).NotEmpty().MaximumLength(300);
        RuleFor(form => form.Priority).NotNull().WithMessage("Priority is empty");
        RuleFor(form => form.Priority).IsInEnum().WithMessage("Value are not allowed");
    }
}
