using Azure.Core;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Todo.Application.Users.Commands.Register;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Domain.Exceptions;
using Todo.Domain.Repositories;
using Todo.Domain.Security;

namespace Todo.Infrastructure.Services;

public class AuthService : IAuthRepository
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IJwtHelper _jwtHelper;
    public AuthService(UserManager<UserEntity> userManager, IJwtHelper jwtHelper)
    {
        _userManager = userManager;
        _jwtHelper = jwtHelper;
    }


    public async Task<string> Login(string email, string password)
    {

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null) throw new ApiErrorException("Bad credentials", 400);

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordCorrect) throw new ApiErrorException("Bad credentials", 400);

        string token = await _jwtHelper.GenerateToken(user);
        return token;
      
    }

    public async Task<bool> Register(UserEntity entity, string password)
    {
        var user = await _userManager.FindByEmailAsync(entity.Email!);
        if (user is not null) throw new ApiErrorException("Bad request", 400);

   
        var result = await _userManager.CreateAsync(entity, password);
        if (!result.Succeeded) throw new ApiErrorException("An error has occurred", 400);
        await _userManager.AddToRoleAsync(entity, UserRole.User);
        IList<Claim> baseClaims = await _jwtHelper.SetBaseClaims(entity);
        var addClaimsResult = await _userManager.AddClaimsAsync(entity, baseClaims);
        if (!addClaimsResult.Succeeded) throw new ApiErrorException("An error has occurred", 400);

        return true;
    }
}
