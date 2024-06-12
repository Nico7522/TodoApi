using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using Todo.Infrastructure.Configs;

namespace Todo.Infrastructure.Persistence;

public class TodoDbContext : IdentityDbContext<UserEntity>
{
    private readonly string _connectionString = "Data Source=DESKTOP-IFNFMI9;Initial Catalog=TodoDB;Integrated Security=True;Connect Timeout=60;Trust Server Certificate=True;";
    public DbSet<TodoEntity> Tasks { get; set; }

    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);

    }

    public TodoDbContext()
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new UserConfig());
        builder.ApplyConfiguration(new TodoConfig());
        builder.ApplyConfiguration(new UserTodoConfig());

    }
}
