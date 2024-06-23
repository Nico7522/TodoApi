using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Configs;

internal class UserConfig : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.Property(u => u.Birthdate).IsRequired();
        builder.Property(u => u.FirstName).IsRequired();
        builder.Property(u => u.LastName).IsRequired();
        builder.Property(u => u.HireDate).IsRequired();
        builder
       .HasMany(e => e.Tasks)
       .WithOne(e => e.User)
       .HasForeignKey(e => e.UserId).IsRequired(false);
        builder.HasOne(e => e.LeadedTeam).WithOne(e => e.Leader).HasForeignKey<TeamEntity>(e => e.LeaderId).IsRequired(false);
        builder.ToTable("AspNetUsers", t => t.HasTrigger("SANoDeleteTrigger"));
    }
}
