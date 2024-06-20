using Todo.Domain.Entities;

namespace Todo.Domain.Repositories;
public interface ITeamRepository : IRepositoryBase
{
    Task<IEnumerable<TeamEntity>> GetAll();
}
