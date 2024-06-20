using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;
using Todo.Application.Email.Interfaces;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;

namespace Todo.Application.Users.Commands.ResetPassword;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IEmailSender _emailSender;
    private readonly UserManager<UserEntity> _userMananger;
    public ResetPasswordCommandHandler(IEmailSender emailSender, UserManager<UserEntity> userManager)
    {
        _emailSender = emailSender;
        _userMananger = userManager;
    }
    public async System.Threading.Tasks.Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userMananger.FindByEmailAsync(request.Email);
        if (user is null) throw new NotFoundException("User not found");

        var resetToken = await _userMananger.GeneratePasswordResetTokenAsync(user);
        var encodedResetToken = HttpUtility.UrlEncode(resetToken);
        await _emailSender.SendEmail(user.Email!, "Reset your password", $"You can reset your password from this url http://localhost:4200/{user.Id}/{encodedResetToken}");
    }
}
