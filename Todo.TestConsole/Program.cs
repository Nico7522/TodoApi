using Microsoft.AspNetCore.Identity;
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
        TodoDbContextTest context = new TodoDbContextTest(configuration);

        try
        {
            var task = context.Tasks.AsNoTracking().Include(t => t.User).Where(t => !t.IsComplete).ToList();
            foreach (var t in task)
            {
                Console.WriteLine(t.Title);
                if(t.User != null)
                {
                    Console.WriteLine(t.User.FirstName);
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
}
