using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using Todo.Infrastructure.Persistence;

namespace Todo.TestConsole;

public class Program
{
    static  void Main(string[] args)
    {
       TodoDbContext context = new TodoDbContext();

		try
		{
			//TodoEntity entity = new TodoEntity()
			//{
			//	Priority = Domain.Enums.Piority.Medium,
			//	Title = "Title",
			//	Description = "Description",
			//};
			// context.Tasks.Add(entity);

   //          Console.Write(entity.Id);

			//var user = context.Users.FirstOrDefault();
			//entity.Users.Add(user);
			//context.SaveChanges();

			//var task = context.Tasks.Include(t => t.Users).FirstOrDefault(t => t.Id.ToString() == "fd78ba2b-e2f2-4d9e-93e5-08dc8ae28662");
   //         Console.WriteLine(task.Title);
   //         foreach (var user in task.Users)
   //         {
   //             Console.WriteLine(user.Id);
   //         }
        }
		catch (Exception)
		{

			throw;
		}
    }
}
