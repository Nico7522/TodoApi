using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Todo.Domain.Entities;
using Todo.Infrastructure.Configs;

namespace Todo.Infrastructure.Persistence;

public class TodoDbContext : IdentityDbContext<UserEntity>
{
    //private readonly IConfiguration _configuration;
    public DbSet<TodoEntity> Tasks { get; set; }

    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {

    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer(_configuration.GetConnectionString("TodoDB"));
    //}

    //public TodoDbContext(IConfiguration configuration)
    //{
    //    _configuration = configuration;
    //}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new UserConfig());
        builder.ApplyConfiguration(new TodoConfig());
        builder.ApplyConfiguration(new UserTodoConfig());

    }
}
