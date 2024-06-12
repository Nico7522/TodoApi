using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;
using System.Reflection.Emit;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Configs;

public class UserTodoConfig : IEntityTypeConfiguration<UserEntity>
{
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder
       .HasMany(e => e.Tasks)
       .WithMany(e => e.Users)
       .UsingEntity("UsersTasks");
    }
}
