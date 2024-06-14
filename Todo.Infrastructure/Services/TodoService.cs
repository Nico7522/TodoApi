using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Services;

internal class TodoService : ITodoRepository
{
    private readonly TodoDbContext _dbContext;
    public TodoService(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Create(TodoEntity entity)
    {
        await _dbContext.Tasks.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity.Id;
    }

    public async Task Delete(TodoEntity entity)
    {
        _dbContext.Tasks.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public Task<IEnumerable<TodoEntity>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<TodoEntity?> GetById(string taskId)
    {
        return await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id.ToString() == taskId);
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }
}
