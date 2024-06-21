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

    public async Task<IEnumerable<TodoEntity>> GetAllActive()
    {
        return await _dbContext.Tasks.AsNoTracking().Include(t => t.User).Where(t => !t.IsComplete).ToListAsync();  
    }

    public async Task<TodoEntity?> GetById(Guid taskId)
    {
        return await _dbContext.Tasks.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task SaveChanges()
    {
        await _dbContext.SaveChangesAsync();
    }
}
