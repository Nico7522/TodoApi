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

    public Task<IEnumerable<TodoEntity>> GetAll()
    {
        throw new NotImplementedException();
    }
}
