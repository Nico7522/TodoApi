using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Todo.Domain.Security;
using Todo.Infrastructure.Persistence;
using Todo.Infrastructure.Security;
using Todo.Infrastructure.Seeders;
using Todo.Infrastructure.Services;

namespace Todo.Infrastructure.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtHelper, JwtHelpers>();
        services.AddScoped<IRoleSeeder, RoleSeeder>();
        services.AddScoped<IAuthRepository, AuthService>();
        services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(configuration["ConnectionStrings:TodoDB"]));
        services.AddIdentityCore<UserEntity>().AddRoles<IdentityRole>().AddEntityFrameworkStores<TodoDbContext>();
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateActor = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))

            };
        });
    }
}
