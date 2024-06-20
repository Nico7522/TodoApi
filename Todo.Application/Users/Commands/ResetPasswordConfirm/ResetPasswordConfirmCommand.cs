using MediatR;

namespace Todo.Application.Users.Commands.ResetPasswordConfirm;

public class ResetPasswordConfirmCommand : IRequest
{
    public string UserId { get; init; }
    public string ResetToken { get; init; }
    public string Password { get; init; }
    public string PasswordConfirm { get; init; }
    public ResetPasswordConfirmCommand(string userId, string resetToken, string password, string passwordConfirm)
    {
        UserId =  userId;
        ResetToken = resetToken;
        Password = password;
        PasswordConfirm = passwordConfirm;
    }
}
