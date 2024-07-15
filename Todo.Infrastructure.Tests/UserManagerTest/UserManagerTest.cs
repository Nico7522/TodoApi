using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Todo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Todo.Domain.Constants;

namespace Todo.Infrastructure.Tests.UserManagerTest;

public class UserManagerTest
{
    private ServiceProvider _serviceProvider;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();
        services.AddDbContext<TodoDbContext>(options =>
               options.UseInMemoryDatabase("TestDb"));
        services.AddDataProtection();
        services.AddIdentityCore<UserEntity>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TodoDbContext>()
            .AddDefaultTokenProviders();


        services.AddTransient<IUserStore<UserEntity>, UserStore<UserEntity, IdentityRole, TodoDbContext>>();
        services.AddTransient<IRoleStore<IdentityRole>, RoleStore<IdentityRole, TodoDbContext>>();
        services.AddTransient<UserManager<UserEntity>>();
        services.AddTransient<RoleManager<IdentityRole>>();

        _serviceProvider = services.BuildServiceProvider();
    }

    [Fact()]
    public async void AddClaim_ForClaimAdded_ShouldAddAndRetrieveClaimCorrectly()
    {

        Setup();
        // Arrange
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var userManager = scopedServices.GetRequiredService<UserManager<UserEntity>>();
            var dbContext = scopedServices.GetRequiredService<TodoDbContext>();

            var user = new UserEntity()
            {
                Id = "id",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@gmail.com",
                PhoneNumber = "491410952",
                Birthdate = new DateOnly(2000, 01, 01),
                UserName = "Test@gmail.com",
            };

            var result = await userManager.CreateAsync(user);

            // Check if the user creation was successful
            if (!result.Succeeded)
            {
                throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var u = await userManager.FindByIdAsync(user.Id);

            await userManager.AddClaimAsync(u, new Claim(ClaimTypes.Role, "SuperAdmin"));
            var claims = await userManager.GetClaimsAsync(u);
            // Assert
            u.Email.Should().Be("Test@gmail.com");
            claims.First().Value.Should().Be("SuperAdmin");
        }


    }

    [Fact()]
    public async void AddRole_ForRoleAdded_ShouldAddAndRetrieveRoleCorrectly()
    {

        Setup();
        // Arrange
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var userManager = scopedServices.GetRequiredService<UserManager<UserEntity>>();
            var dbContext = scopedServices.GetRequiredService<TodoDbContext>();

            var user = new UserEntity()
            {
                Id = "id",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@gmail.com",
                PhoneNumber = "491410952",
                Birthdate = new DateOnly(2000, 01, 01),
                UserName = "Test@gmail.com",
            };

            var result = await userManager.CreateAsync(user);
            await dbContext.Roles.AddAsync(new(UserRole.User)
            {
                NormalizedName = UserRole.User.ToUpper()
            });
            await dbContext.SaveChangesAsync();
            // Check if the user creation was successful
            if (!result.Succeeded)
            {
                throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }


            var u = await userManager.FindByIdAsync(user.Id);
            await userManager.AddToRoleAsync(u, "User");
            var roles = await userManager.GetRolesAsync(u);
            // Assert
            roles.Should().HaveCount(1);
            roles.First().Should().Be("User");
        }



    }

    [Fact()]
    public async void Get_ForGetUserTasksCount_ShouldReturnTasksCountCorrectly()
    {

        Setup();
        // Arrange
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var userManager = scopedServices.GetRequiredService<UserManager<UserEntity>>();
            var dbContext = scopedServices.GetRequiredService<TodoDbContext>();

            var user = new UserEntity()
            {
                Id = "id",
                FirstName = "Test",
                LastName = "Test",
                Email = "Test@gmail.com",
                PhoneNumber = "491410952",
                Birthdate = new DateOnly(2000, 01, 01),
                UserName = "Test@gmail.com",
            };
            var result = await userManager.CreateAsync(user);
            await dbContext.SaveChangesAsync();
            var task1 = new TodoEntity() { Title = "Test", Description = "test" };
            var task2 = new TodoEntity() { Title = "Test", Description = "test" };
            var u = await userManager.FindByIdAsync(user.Id);
            u.Tasks.Add(task1);
            u.Tasks.Add(task2);

            var userWithTasks = await dbContext.Users.Include(u => u.Tasks).FirstOrDefaultAsync(u => u.Id == user.Id);
            var tasksCount = userWithTasks.Tasks.Count();

            // Assert

            tasksCount.Should().Be(2);
        }



    }
}