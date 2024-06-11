using Microsoft.AspNetCore.Identity;
using Todo.Domain.Entities;
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
        if (user is null) return null;

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
        if (!isPasswordCorrect) return null;

        string token = await _jwtHelper.GenerateToken(user);

        return token;
    }

    public Task<bool> Register()
    {
        throw new NotImplementedException();
    }
}
