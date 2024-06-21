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
            var team = context.Teams.FirstOrDefault(t => t.Id.ToString() == "aeb460ae-7c2c-4b7a-1a3a-08dc8c4c02a9");
            var task = context.Tasks.FirstOrDefault(t => t.Id.ToString() == "046b88e6-0320-4db7-7dd7-08dc8c80cbbc");
            if (team is not null && task is not null)
            {
                team.Tasks.Add(task);
                context.SaveChanges();
                Console.WriteLine("ok");
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
}
