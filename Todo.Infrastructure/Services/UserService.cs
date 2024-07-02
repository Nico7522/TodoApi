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

    public Task<int> GetTaskNumberByUser()
    {
        throw new NotImplementedException();
    }
}
