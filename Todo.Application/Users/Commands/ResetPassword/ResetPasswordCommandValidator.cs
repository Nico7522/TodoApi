

using FluentValidation;

namespace Todo.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(f => f.Email).NotEmpty()
            .WithMessage("Email field required")
            .EmailAddress()
            .WithMessage("Email is not valid");
    }
}
