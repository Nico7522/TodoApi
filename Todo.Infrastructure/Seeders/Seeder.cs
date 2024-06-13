using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Constants;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Seeders;

internal class Seeder : ISeeder
{
    private readonly TodoDbContext _dbContext;
    public Seeder(TodoDbContext dbContext)
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

            if (!_dbContext.Tasks.Any())
            {
                var tasks = GetTasks();
                _dbContext.Tasks.AddRange(tasks);
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

    private IEnumerable<TodoEntity> GetTasks()
    {
        List<TodoEntity> tasks = new List<TodoEntity>()
        {
            new()
            {
                Title = "Faire les courses",
                Description = "Allez au magasin",
                CreationDate = new DateOnly(),
                Priority = Domain.Enums.Piority.Medium
            },
            new()
            {
                Title = "Tondre la pelouse",
                Description = "Avant 16h, tondre la pelouse",
                CreationDate = new DateOnly(),
                Priority = Domain.Enums.Piority.High
            },
            new()
            {
                Title = "Nettoyer la cuisine",
                Description = "Nettoyer du sol au plafond !!",
                CreationDate = new DateOnly(),
                Priority = Domain.Enums.Piority.Urgent
            },
            new()
            {
                Title = "Faire un gâteau",
                Description = "Gateau au chocolat",
                CreationDate = new DateOnly(),
                Priority = Domain.Enums.Piority.Low
            },
            new()
            {
                Title = "Dormir tôt",
                Description = "Plus d'idée",
                CreationDate = new DateOnly(),
                Priority = Domain.Enums.Piority.High
            }
        };

        return tasks;
    }


}
