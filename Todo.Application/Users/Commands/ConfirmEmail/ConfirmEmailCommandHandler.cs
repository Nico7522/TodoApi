using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;

namespace Todo.Application.Users.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand>
{
    private readonly UserManager<UserEntity> _userManager;
    public ConfirmEmailCommandHandler(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }
    public async System.Threading.Tasks.Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId is null || request.Token is null) throw new BadRequestException("Link expired");

        var user = await _userManager.FindByIdAsync(request.UserId);

        if (user is null) throw new NotFoundException("User not found");


        //var token = HttpUtility.UrlDecode(request.Token);
        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded) throw new BadRequestException("Email could not be confirmed");
    }
}
