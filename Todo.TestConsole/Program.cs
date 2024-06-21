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
            var user = context.Users.FirstOrDefault(t => t.Id == "0d186c59-c16c-4b3a-bbe2-2ebbdcf0c898");
            if (team is not null && user is not null)
            {
                team.Users.Add(user);
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
