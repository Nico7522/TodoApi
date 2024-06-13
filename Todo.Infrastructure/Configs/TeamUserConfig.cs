using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Configs;

public class TeamUserConfig : IEntityTypeConfiguration<TeamEntity>
{
    public void Configure(EntityTypeBuilder<TeamEntity> builder)
    {
        builder.HasMany(e => e.Users).WithOne(e => e.Team).HasForeignKey(e => e.TeamId).IsRequired(false);
    }
}
