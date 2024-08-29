using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Todo.Application.Users;
using Todo.Domain.Entities;
using Todo.Domain.Security;

namespace Todo.Infrastructure.Security;

public class JwtHelpers : IJwtHelper
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly IConfiguration _configuration;
    public JwtHelpers(UserManager<UserEntity> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<string> GenerateToken(UserEntity user)
    {
        var claims = await _userManager.GetClaimsAsync(user);

        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
        SigningCredentials signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        SecurityToken securityToken = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(3),
                issuer: _configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                signingCredentials: signingCred
                );
        string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
        return tokenString;
    }

    public async Task<IList<Claim>> SetBaseClaims(UserEntity user)
    {
        var role = await  _userManager.GetRolesAsync(user);
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, role.First()),
            new Claim("Firstname", user.FirstName),
            new Claim("Lastname", user.LastName),

        };

        return claims;
    }

  
}
