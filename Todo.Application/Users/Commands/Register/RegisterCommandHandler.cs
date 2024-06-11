
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Security;

namespace Todo.Application.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IJwtHelper _jwtHelper;

    public RegisterCommandHandler(UserManager<UserEntity> userManager, IJwtHelper jwtHelper)
    {
        _userManager = userManager;
        _jwtHelper = jwtHelper;
    }
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await  _userManager.FindByEmailAsync(request.Email);
        if (user is not null) return false;


        // TODO: faire un mapper.
        UserEntity entity = new UserEntity()
        {
            Email = request.Email,
            Birthdate = request.BirthDate,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
        };

        var result = await _userManager.CreateAsync(entity, request.Password);
        if (!result.Succeeded) return false;
        await _userManager.AddToRoleAsync(entity, UserRole.User);
        IList<Claim> baseClaims = await _jwtHelper.SetBaseClaims(entity);
        var addClaimsResult = await _userManager.AddClaimsAsync(entity, baseClaims);
        if(!addClaimsResult.Succeeded) return false;

        return true;    



    }
}
