using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
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
            //var userWithMostTasks = context.Tasks.Where(t => t.UserId != null)
            //            .GroupBy(t => t.UserId)
            //            .Select(group => new
            //            {
            //                User = group.Key,
            //                TaskCount = group.Count()
            //            })
            //                .OrderByDescending(x => x.TaskCount);

            //foreach (var item in userWithMostTasks)
            //{
            //    Console.WriteLine(item.TaskCount);
            //}

            var test = context.Tasks.Where(t => t.TeamId != null).GroupBy(t => t.TeamId).Select(g => new
            {
                TeamId = g.Key,
                Tasks = g.ToList()
            }).ToList();

            foreach (var t in test)
            {
                Console.WriteLine(t.TeamId);
                foreach (var item in t.Tasks)
                {
                    Console.WriteLine(item.Title);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }
    }
}
