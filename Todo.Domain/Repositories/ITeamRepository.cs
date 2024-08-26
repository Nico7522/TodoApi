using Todo.Domain.Entities;
namespace Todo.Domain.Repositories;
public interface ITeamRepository : IRepositoryBase
{
    Task<IEnumerable<TeamEntity>> GetAll(bool isActive);
    Task<TeamEntity?> GetById(Guid id);

    Task<Guid> Create(TeamEntity entity);
}
