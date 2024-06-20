
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Configs;

public class TeamConfig : IEntityTypeConfiguration<TeamEntity>
{
    public void Configure(EntityTypeBuilder<TeamEntity> builder)
    {
        builder.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
        builder.HasMany(e => e.Tasks).WithOne(e => e.Team).HasForeignKey(e => e.TeamId).IsRequired(false);
    }
}
