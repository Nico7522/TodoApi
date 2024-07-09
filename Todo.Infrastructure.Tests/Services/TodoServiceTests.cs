using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Todo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Todo.Domain.Repositories;
using Todo.Domain.Entities;
using FluentAssertions;

namespace Todo.Infrastructure.Services.Tests;

public class TodoServiceTests
{
    private ServiceProvider _serviceProvider;

    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddDbContext<TodoDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        services.AddScoped<ITodoRepository, TodoService>();

        _serviceProvider = services.BuildServiceProvider();
    }
    [Fact()]
    public async void Create_ForValidCommand_ShouldCreateTaskCorrectly()
    {

        Setup();
        // Arrange
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var service = scopedServices.GetRequiredService<ITodoRepository>();
            var dbContext = scopedServices.GetRequiredService<TodoDbContext>();

            var task = new TodoEntity()
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                Description = "Test",
                Priority = Domain.Enums.Priority.Low
            };

            // Act
            var result = await service.Create(task);

            // Assert
            var addedItem = dbContext.Tasks.Find(task.Id);
            addedItem.Should().NotBeNull();
            addedItem.Title.Should().Be(task.Title);
        }
    }
}