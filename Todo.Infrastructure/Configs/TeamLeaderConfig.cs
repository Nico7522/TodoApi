using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Configs;

public class TeamLeaderConfig : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.HasOne(e => e.LeadedTeam).WithOne(e => e.Leader).HasForeignKey<TeamEntity>(e => e.LeaderId).IsRequired(false);
    }
}
