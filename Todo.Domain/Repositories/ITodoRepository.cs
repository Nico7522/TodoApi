﻿using Todo.Domain.Entities;

namespace Todo.Domain.Repositories;

public interface ITodoRepository : IRepositoryBase
{
    Task<IEnumerable<TodoEntity>> GetAll();
    Task<TodoEntity?> GetById(string taskId);
    Task Delete(TodoEntity entity);
    Task<Guid> Create(TodoEntity entity);
}
