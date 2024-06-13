using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence;

namespace Todo.TestConsole;

public class Program
{
    static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();
        //TodoDbContext context = new TodoDbContext(configuration);

        //try
        //{
        //    var team = context.Teams.Include(t => t.Tasks).FirstOrDefault();

        //    foreach (var task in team.Tasks)
        //    {
        //        Console.WriteLine(task.Title);
        //    }

        //    Console.WriteLine("ok");

        //}
        //catch (Exception)
        //{

        //    throw;
        //}
    }
}
