using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Todo.Domain.Entities;
using Todo.Domain.Repositories;
using Todo.Infrastructure.Persistence;
using Xunit;


namespace Todo.Infrastructure.Services.Tests;

public class TeamServiceTests
{
    private ServiceProvider _serviceProvider;
    [TestInitialize]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddDbContext<TodoDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        services.AddScoped<ITeamRepository, TeamService>();
        _serviceProvider = services.BuildServiceProvider();
    }
    [Fact()]
    public async Task GetAll_ForValidQuery_ShouldReturnAllActiveTeam()
    {
        Setup();
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var service = scopedServices.GetRequiredService<ITeamRepository>();
            var dbContext = scopedServices.GetRequiredService<TodoDbContext>();

            var team1 = new TeamEntity() { Name = "team", IsActive = true };
            var team2 = new TeamEntity() { Name = "team", IsActive = true };
            var team3 = new TeamEntity() { Name = "team", IsActive = false };


            ICollection<TeamEntity> teams = new[] { team1, team2, team3 };
            await dbContext.Teams.AddRangeAsync(teams);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await service.GetAll(true);

            // Assert
            result.Should().HaveCount(2);
        }
    }

    [Fact()]
    public async Task GetAll_ForValidQuery_ShouldReturnAllNoActiveTeam()
    {
        Setup();
        using (var scope = _serviceProvider.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var service = scopedServices.GetRequiredService<ITeamRepository>();
            var dbContext = scopedServices.GetRequiredService<TodoDbContext>();

            var team1 = new TeamEntity() { Name = "team", IsActive = true };
            var team2 = new TeamEntity() { Name = "team", IsActive = true };
            var team3 = new TeamEntity() { Name = "team", IsActive = false };


            ICollection<TeamEntity> teams = new[] { team1, team2, team3 };
            await dbContext.Teams.AddRangeAsync(teams);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await service.GetAll(false);

            // Assert
            result.Should().HaveCount(1);
        }
    }
}