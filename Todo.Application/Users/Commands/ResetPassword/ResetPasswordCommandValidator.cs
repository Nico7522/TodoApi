using FluentValidation;
using Todo.Application.Utility;

namespace Todo.Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(f => f.Email).NotEmpty()
            .NotEmpty()
            .WithMessage(Helpers.SetRequiredErrorMessage("Email"))
            .EmailAddress()
            .WithMessage("Email is not valid")
            .Matches(@"^[\w -\.]+@([\w-]+\.)+[\w-]{2,4}$").WithMessage("Email is not valid");
    }
}
