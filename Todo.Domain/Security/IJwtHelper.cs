using System.Security.Claims;
using Todo.Domain.Entities;

namespace Todo.Domain.Security;

public interface IJwtHelper
{
    public Task<string> GenerateToken(UserEntity user);
    public Task<IList<Claim>> SetBaseClaims(UserEntity user);
}
