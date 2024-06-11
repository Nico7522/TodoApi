
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Entities;

namespace Todo.Infrastructure.Configs;

public class TodoConfig : IEntityTypeConfiguration<TodoEntity>
{
    public void Configure(EntityTypeBuilder<TodoEntity> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.Property(t => t.Title).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Description).IsRequired();
        builder.Property(t => t.CreationDate).HasDefaultValue(new DateOnly());
        builder.Property(t => t.Priority).IsRequired();
        builder.Property(t => t.IsComplete).HasDefaultValue(false);
    }
}
