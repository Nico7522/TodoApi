using Todo.Domain.Entities;

namespace Todo.Domain.Repositories;

public interface ITodoRepository
{
    Task<IEnumerable<TodoEntity>> GetAll();
    Task<TodoEntity?> GetById(string taskId);
}
