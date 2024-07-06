using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Services;

internal class UserService : IUserRepository
{
    private readonly TodoDbContext _dbContext;
    private readonly UserManager<UserEntity> _userManager;
    public UserService(TodoDbContext dbContext, UserManager<UserEntity> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public Task<int> GetTaskNumberByUser()
    {
        throw new NotImplementedException();
    }

    public async Task<UserEntity?> GetUserWithTeam(string userId)
    {
        var user = await _dbContext.Users.Include(u => u.Team).FirstOrDefaultAsync(u => u.Id == userId);
        return user;
    }
}
