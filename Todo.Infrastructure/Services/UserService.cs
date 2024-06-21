using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Services;

internal class UserService : IUserRepository
{
    private readonly TodoDbContext _dbContext;
    public UserService(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> AssignTaskByUser(UserEntity user, TodoEntity task)
    {
        task.User = user;
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<UserEntity?> GetTasksByUser(string userId)
    {
        return await _dbContext.Users.AsNoTracking().Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == userId);
    }
}
