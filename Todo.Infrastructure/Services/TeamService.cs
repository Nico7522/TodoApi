﻿using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Persistence;

namespace Todo.Infrastructure.Services;

internal class TeamService : ITeamRepository
{
    private readonly TodoDbContext _dbContext;
    public TeamService(TodoDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<TeamEntity>> GetAll()
    {
        return await _dbContext.Teams.AsNoTracking().ToListAsync();
    }

    public Task SaveChanges()
    {
        throw new NotImplementedException();
    }
}
