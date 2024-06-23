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
            var user = context.Users.Include(u => u.Tasks).FirstOrDefault(t => t.Email == "nico.daddabbo2000@gmail.com");
            //foreach (var t in user.Tasks)
            //{
            //    user.Tasks.Remove(t);
            //    context.SaveChanges();
            //}
            context.Users.Remove(user);
            context.SaveChanges();
            Console.WriteLine("ok");

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}
