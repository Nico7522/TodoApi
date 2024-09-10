using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;

namespace Todo.Application.Users.Commands.ResetPasswordConfirm;

public class ResetPasswordConfirmCommandHandler : IRequestHandler<ResetPasswordConfirmCommand>
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IValidator<ResetPasswordConfirmCommand> _validator;
    public ResetPasswordConfirmCommandHandler(UserManager<UserEntity> userManager, IValidator<ResetPasswordConfirmCommand> validator)
    {
        _userManager = userManager;
        _validator = validator;
    }
    public async System.Threading.Tasks.Task Handle(ResetPasswordConfirmCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (validationResult.Errors.Any())
        {
            throw new ValidationException(validationResult.Errors);
        }

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null) throw new NotFoundException("User not found");

        var decodedResetToken = HttpUtility.UrlDecode(request.ResetToken).Replace(" ", "+");

        var result = await _userManager.ResetPasswordAsync(user, decodedResetToken, request.Password);
        if (!result.Succeeded) throw new ApiException("A error has occured");


    }
}
