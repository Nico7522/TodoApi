
using FluentValidation;

namespace Todo.Application.Users.Commands.ResetPasswordConfirm;

public class ResetPasswordConfirmCommandValidator : AbstractValidator<ResetPasswordConfirmCommand>
{
    public ResetPasswordConfirmCommandValidator()
    {
        RuleFor(c => c.ResetToken).NotEmpty().WithMessage("Token is empty");
        RuleFor(c => c.UserId).NotEmpty().WithMessage("User id is empty");
        RuleFor(c => c.Password).NotEmpty().WithMessage("Password field required")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&=#])[A-Za-z\d@$!%*?&=#]{8,}$")
            .WithMessage("Password not match the requirement");
        RuleFor(c => c.Password).Equal(c => c.PasswordConfirm).WithMessage("Password not match");
        RuleFor(c => c.PasswordConfirm).NotEmpty().WithMessage("Password confirm field required");
          
    }
}
