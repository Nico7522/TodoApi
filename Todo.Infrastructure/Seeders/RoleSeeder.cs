using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Constants;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Seeders;

internal class RoleSeeder : IRoleSeeder
{
    private readonly TodoDbContext _dbContext;
    public RoleSeeder(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Seed()
    {
        if (await _dbContext.Database.CanConnectAsync())
        {
            if (!_dbContext.Roles.Any())
            {
                var roles = GetRoles();
                _dbContext.Roles.AddRange(roles);
                await _dbContext.SaveChangesAsync();
            }
        }
    }


    private IEnumerable<IdentityRole> GetRoles()
    {
        List<IdentityRole> roles = new List<IdentityRole>()
        {
            new (UserRole.User)
            {
                NormalizedName = UserRole.User.ToUpper()
            },
            new(UserRole.Leader)
            {
                NormalizedName= UserRole.Leader.ToUpper()
            },
            new(UserRole.Admin)
            {
                NormalizedName = UserRole.Admin.ToUpper()
            },
            new(UserRole.SuperAdmin)
            {
                NormalizedName = UserRole.SuperAdmin.ToUpper()
            },
        };

        return roles;
    }
}
