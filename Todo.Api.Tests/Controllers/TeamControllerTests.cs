using Todo.Api.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using System.Net.Http.Json;
using Todo.Api.Tests;
using Todo.Application.Team.Dto;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Xunit;
using Todo.Application.Team.Commands.CreateTeam;
using Newtonsoft.Json;
using System.Text;
using MediatR;
using Todo.Api.Forms.UpdateTaskByTeamForm;
using Todo.Application.Team.Commands.UnassignTaskFromTeam;
using Todo.Api.Forms.CompleteTaskByTeamForm;


namespace Todo.Api.Controllers.Tests;

public class TeamControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly TeamController _teamController;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IAuthorization<TeamEntity>> _authMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();

    public TeamControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.Replace(ServiceDescriptor.Scoped(typeof(IAuthorization<TeamEntity>), _ => _authMock.Object));
            });
        });
        _teamController = new TeamController(_mediatorMock.Object);
    }
    [Fact()]
    public async void GetAllActiveTeams_ForValidRequest_Return200OkWithTeams()
    {
        // arrange

        var client = _factory.CreateClient();

        // act

        var result = await client.GetAsync("api/team?isActive=true");
        var content = await client.GetFromJsonAsync<IEnumerable<TeamDto>>("api/team?isActive=true");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().HaveCount(3);
    }

    [Fact()]
    public async void GetAllUnactiveTeams_ForValidRequest_Return200OkWithTeams()
    {
        // arrange

        var client = _factory.CreateClient();

        // act

        var result = await client.GetAsync("api/team?isActive=false");
        var content = await client.GetFromJsonAsync<IEnumerable<TeamDto>>("api/team?isActive=false");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().HaveCount(2);
    }

    [Fact()]
    public async void GetTeamById_ForValidRequest_Return200OkWithTeam()
    {
        // arrange

        var client = _factory.CreateClient();
        var id = new Guid("aeb460ae-7c2c-4b7a-1a3a-08dc8c4c02a9");
        // act

        var result = await client.GetAsync($"api/team/{id}");
        var content = await client.GetFromJsonAsync<TeamDto>($"api/team/{id}");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Name.Should().Be("Team rouge");
    }

    [Fact()]
    public async void GetTeamById_ForInvalidRequest_Return404ONotFound()
    {
        // arrange

        var client = _factory.CreateClient();
        var id = new Guid("aeb460ae-7c2c-4b7a-1a3a-08dc8c4c02c9");
        // act

        var result = await client.GetAsync($"api/team/{id}");
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void Create_ForValidRequest_Return201Created()
    {
        // arrange

        var client = _factory.CreateClient();
        var content = new CreateTeamCommand("test");
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        var result = await client.PostAsync($"api/team", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact()]
    public async void Create_ForInvalidRequest_Return400BaDRequest()
    {
        // arrange

        var client = _factory.CreateClient();
        var content = new CreateTeamCommand("");
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        var result = await client.PostAsync($"api/team", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void CloseTeam_ForValidCommand_Return200Ok()
    {
        // arrange

        var id = new Guid("aeb460ae-7c2c-4b7a-1a3a-08dc8c4c02a9");
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{id}/close", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact()]
    public async void CloseTeam_ForInvalidCommand_Return404NotFound()
    {
        // arrange

        var id = new Guid("d9061321-1c80-4bb4-0e4c-08dc9f5c9d50");
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{id}/close", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void CloseTeam_ForInvalidCommand_Return400BadRequest()
    {
        // arrange

        var id = new Guid("d9061321-1c80-4bb4-0e4c-08dc9f5c8d50");
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{id}/close", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void AssignUserByTeam_ForValidCommand_Return200Ok()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);
        // act

        var result = await client.PostAsync($"api/team/{teamId}/user/{userId}", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact()]
    public async void AssignUserByTeam_ForInvalidCommandTeamNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bc8");
        var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);
        // act

        var result = await client.PostAsync($"api/team/{teamId}/user/{userId}", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void AssignUserByTeam_ForInvalidCommandUserNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "47c03a1a-9838-4484-8126-c1d02f90892a";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);
        // act

        var result = await client.PostAsync($"api/team/{teamId}/user/{userId}", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void AssignUserByTeam_ForInvalidCommandUserAlreadyInTeam_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);

        // act

        var result = await client.PostAsync($"api/team/{teamId}/user/{userId}", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void AssignUserByTeam_ForInvalidCommandUserAlreadyInAnotherTeam_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "1869c5aa-6109-464c-acfc-7cba0bb0b687";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);

        // act

        var result = await client.PostAsync($"api/team/{teamId}/user/{userId}", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void AssignUserByTeam_ForInvalidCommandTeamIsInactive_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("ce32b1d7-98bb-410c-505e-08dca59820c4");
        var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);

        // act

        var result = await client.PostAsync($"api/team/{teamId}/user/{userId}", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void DeleteUserFromTeam_ForValidCommand_Return200Ok()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Delete)).Returns(true);

        // act

        var result = await client.DeleteAsync($"api/team/{teamId}/user/{userId}");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact()]
    public async void DeleteUserFromTeam_ForInvalidCommandTeamNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5726bf8");
        var userId = "1869c5aa-6109-464c-acfc-7cba0bb0b687";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Delete)).Returns(true);

        // act

        var result = await client.DeleteAsync($"api/team/{teamId}/user/{userId}");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }


    [Fact()]
    public async void DeleteUserFromTeam_ForInvalidCommandUserNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "1869c5aa-6109-464c-acfc-7cba0bb0c687";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Delete)).Returns(true);

        // act

        var result = await client.DeleteAsync($"api/team/{teamId}/user/{userId}");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void DeleteUserFromTeam_ForInvalidCommandUserIsLeader_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("ce32b1d7-98bb-410c-505e-08dca59820c4");
        var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Delete)).Returns(true);

        // act

        var result = await client.DeleteAsync($"api/team/{teamId}/user/{userId}");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void DeleteUserFromTeam_ForInvalidCommandUserNotInTeam_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("ce32b1d7-98bb-410c-505e-08dca59820c4");
        var userId = "1869c5aa-6109-464c-acfc-7cba0bb0b687";
        var client = _factory.CreateClient();
        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Delete)).Returns(true);

        // act

        var result = await client.DeleteAsync($"api/team/{teamId}/user/{userId}");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void AssignLeaderByTeam_ForValidRequest_Return200Ok()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{teamId}/user/{userId}/assignleader", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }


    [Fact()]
    public async void AssignLeaderByTeam_ForInvalidRequestTeamNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5936bf8");
        var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{teamId}/user/{userId}/assignleader", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void AssignLeaderByTeam_ForInvalidRequestUserNotFound_Return404notFound()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "e4c3d048-252e-450a-be51-7b2edbf5eda6";
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{teamId}/user/{userId}/assignleader", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }


    [Fact()]
    public async void AssignLeaderByTeam_ForInvalidRequestUserAlreadyLeaderOfTeam_Return400badRequest()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{teamId}/user/{userId}/assignleader", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void AssignLeaderByTeam_ForInvalidRequestUserAlreadyInAnotherTeam_Return400badRequest()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "1869c5aa-6109-464c-acfc-7cba0bb0b687";
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{teamId}/user/{userId}/assignleader", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void AssignLeaderByTeam_ForInvalidRequestTeamHasAlreadyALeader_Return400badRequest()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{teamId}/user/{userId}/assignleader", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void UnassignLeaderFromTeam_ForValidRequest_Return200Ok()
    {
        // arrange

        var teamId = new Guid("c01dc901-b6b2-403e-52e4-08dc9c1b33a9");
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{teamId}/unassignleader", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact()]
    public async void UnassignLeaderFromTeam_ForInvalidRequestTeamNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("c01dc901-b6b2-403e-52e4-08dc9c1b73a9");
        var client = _factory.CreateClient();

        // act

        var result = await client.PutAsync($"api/team/{teamId}/unassignleader", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void UnassignLeaderFromTeam_ForInvalidRequestTeamLeaderNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("c01dc901-b6b2-403e-52e4-08dc9c1b33a9");
        var client = _factory.CreateClient();

        // act
        Func<Task> act = async () => await _teamController.UnassignLeaderFromTeam(teamId);
        var result = await client.PutAsync($"api/team/{teamId}/unassignleader", null);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void AddTaskToTeam_ForValidRequest_Return200Ok()
    {
        // arrange

        var teamId = new Guid("2c53d5f8-5cc2-4d51-96f4-08dc91ea9f78");
        var taskId = new Guid("9382f33f-acf8-431d-55b3-08dca4d8e649");
        var client = _factory.CreateClient();

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);
        var result = await client.PostAsync($"api/team/{teamId}/task/{taskId}", null);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact()]
    public async void AddTaskToTeam_ForInvalidRequestTaskNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("2c53d5f8-5cc2-4d51-96f4-08dc91ea9f78");
        var taskId = new Guid("9382f33f-acf8-431d-55b3-07dca4d8e649");
        var client = _factory.CreateClient();

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);
        var result = await client.PostAsync($"api/team/{teamId}/task/{taskId}", null);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void AddTaskToTeam_ForInvalidRequestTeamNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("2b53d5f8-5cc2-4d51-96f4-08dc91ea9f78");
        var taskId = new Guid("9382f33f-acf8-431d-55b3-08dca4d8e649");
        var client = _factory.CreateClient();

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);
        var result = await client.PostAsync($"api/team/{teamId}/task/{taskId}", null);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void AddTaskToTeam_ForInvalidRequestTeamNotActive_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("ce32b1d7-98bb-410c-505e-08dca59820c4");
        var taskId = new Guid("9382f33f-acf8-431d-55b3-08dca4d8e649");
        var client = _factory.CreateClient();

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);
        var result = await client.PostAsync($"api/team/{teamId}/task/{taskId}", null);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }


    [Fact()]
    public async void AddTaskToTeam_ForInvalidRequestUnauthorizedAction_Return403Forbiden()
    {
        // arrange

        var teamId = new Guid("2c53d5f8-5cc2-4d51-96f4-08dc91ea9f78");
        var taskId = new Guid("9382f33f-acf8-431d-55b3-08dca4d8e649");
        var client = _factory.CreateClient();

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(false);
        var result = await client.PostAsync($"api/team/{teamId}/task/{taskId}", null);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact()]
    public async void AddTaskToTeam_ForInvalidRequestTaskAlreadyInTeam_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("2c53d5f8-5cc2-4d51-96f4-08dc91ea9f78");
        var taskId = new Guid("9382f33f-acf8-431d-55b3-08dca4d8e649");
        var client = _factory.CreateClient();

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);
        var result = await client.PostAsync($"api/team/{teamId}/task/{taskId}", null);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void AddTaskToTeam_ForInvalidRequestTaskAlreadyAssigned_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("6d11554a-0ef1-4f15-3a1c-08dca5926bf8");
        var taskId = new Guid("9382f33f-acf8-431d-55b3-08dca4d8e649");
        var client = _factory.CreateClient();

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Create)).Returns(true);
        var result = await client.PostAsync($"api/team/{teamId}/task/{taskId}", null);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void UpdateTaskByTeam_ForValidRequest_Return200Ok()
    {
        // arrange

        var teamId = new Guid("2c53d5f8-5cc2-4d51-96f4-08dc91ea9f78");
        var taskId = new Guid("9f150232-005e-43b2-3419-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new UpdateTaskByTeamForm() { Description = "Test", Priority = Domain.Enums.Priority.Urgent, Title = "test" };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact()]
    public async void UpdateTaskByTeam_ForInvalidRequestTeamNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("2c53d5f8-5cc2-4d51-96f4-08dc91ee9f78");
        var taskId = new Guid("9f150232-005e-43b2-3419-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new UpdateTaskByTeamForm() { Description = "Test", Priority = Domain.Enums.Priority.Urgent, Title = "test" };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void UpdateTaskByTeam_ForInvalidRequestTaskNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("2c53d5f8-5cc2-4d51-96f4-08dc91ea9f78");
        var taskId = new Guid("9f150232-005e-43b2-3419-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new UpdateTaskByTeamForm() { Description = "Test", Priority = Domain.Enums.Priority.Urgent, Title = "test" };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void UpdateTaskByTeam_ForInvalidRequestTaskNotInTeam_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("aeb460ae-7c2c-4b7a-1a3a-08dc8c4c02a9");
        var taskId = new Guid("b3232cb2-1b59-4b19-3418-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new UpdateTaskByTeamForm() { Description = "Test", Priority = Domain.Enums.Priority.Urgent, Title = "test" };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void UpdateTaskByTeam_ForInvalidRequestNotAuthorized_Return401Forbidden()
    {
        // arrange

        var teamId = new Guid("2c53d5f8-5cc2-4d51-96f4-08dc91ea9f78");
        var taskId = new Guid("9f150232-005e-43b2-3419-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new UpdateTaskByTeamForm() { Description = "Test", Priority = Domain.Enums.Priority.Urgent, Title = "test" };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(false);
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact()]
    public async void UpdateTaskByTeam_ForInvalidRequestInvalidForm_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("2c53d5f8-5cc2-4d51-96f4-08dc91ea9f78");
        var taskId = new Guid("9f150232-005e-43b2-3419-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new UpdateTaskByTeamForm() { Description = "Test", Priority = Domain.Enums.Priority.Urgent, Title = "" };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void UnassignTaskFromTeam_ForValidRequest_Return204NoContent()
    {
        // arrange

        var teamId = new Guid("0f42c786-9d05-439d-4c0b-08dcc35177cb");
        var taskId = new Guid("63199241-decd-4cce-a30a-08dcc1d13e72");
        var client = _factory.CreateClient();
        var content = new UnassignTaskFromTeamCommand(teamId, taskId);

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var result = await client.DeleteAsync($"api/team/{teamId}/task/{taskId}");
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact()]
    public async void UnassignTaskFromTeam_ForInvalidRequestTeamNotFound_Return404notFound()
    {
        // arrange

        var teamId = new Guid("0f42c786-9d05-439d-4c0b-08dcc35177cf");
        var taskId = new Guid("63199241-decd-4cce-a30a-08dcc1d13e72");
        var client = _factory.CreateClient();
        var content = new UnassignTaskFromTeamCommand(teamId, taskId);

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var result = await client.DeleteAsync($"api/team/{teamId}/task/{taskId}");
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void UnassignTaskFromTeam_ForInvalidRequestTaskNotInTeam_Return404notFound()
    {
        // arrange

        var teamId = new Guid("0f42c786-9d05-439d-4c0b-08dcc35177cb");
        var taskId = new Guid("63199241-decd-4cce-a30a-08dcc1d13e73");
        var client = _factory.CreateClient();
        var content = new UnassignTaskFromTeamCommand(teamId, taskId);

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Delete)).Returns(true);
        var result = await client.DeleteAsync($"api/team/{teamId}/task/{taskId}");
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void UnassignTaskFromTeam_ForInvalidRequestNotAuthorized_Return401Forbidden()
    {
        // arrange

        var teamId = new Guid("0f42c786-9d05-439d-4c0b-08dcc35177cb");
        var taskId = new Guid("92986c5a-6b77-4e14-7b96-08dca4d88c66");
        var client = _factory.CreateClient();
        var content = new UnassignTaskFromTeamCommand(teamId, taskId);

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Delete)).Returns(false);
        var result = await client.DeleteAsync($"api/team/{teamId}/task/{taskId}");
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact()]
    public async void CompleteTaskByTeam_ForValidRequest_Return204NoContent()
    {
        // arrange

        var teamId = new Guid("0f42c786-9d05-439d-4c0b-08dcc35177cb");
        var taskId = new Guid("63199241-decd-4cce-a30a-08dcc1d13e72");
        var client = _factory.CreateClient();
        var content = new CompleteTaskByTeamForm() { Duration = 10};

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}/complete", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact()]
    public async void CompleteTaskByTeam_ForInalidRequestTeamNotFound_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("0f42c786-9d05-439d-4c0b-08dcc35177cd");
        var taskId = new Guid("63199241-decd-4cce-a30a-08dcc1d13e72");
        var client = _factory.CreateClient();
        var content = new CompleteTaskByTeamForm() { Duration = 10 };

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}/complete", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }


    [Fact()]
    public async void CompleteTaskByTeam_ForInvalidRequestTaskNotIntTeam_Return404NotFound()
    {
        // arrange

        var teamId = new Guid("0f42c786-9d05-439d-4c0b-08dcc35177cb");
        var taskId = new Guid("63199241-decd-4cce-a30a-08dcc1d13e73");
        var client = _factory.CreateClient();
        var content = new CompleteTaskByTeamForm() { Duration = 10 };

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}/complete", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void CompleteTaskByTeam_ForInvalidRequestTaskIsAlreadyCompleted_Return400BadRequest()
    {
        // arrange

        var teamId = new Guid("0f42c786-9d05-439d-4c0b-08dcc35177cb");
        var taskId = new Guid("63199241-decd-4cce-a30a-08dcc1d13e72");
        var client = _factory.CreateClient();
        var content = new CompleteTaskByTeamForm() { Duration = 10 };

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}/complete", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void CompleteTaskByTeam_ForInvalidRequestNotAuthorized_Return403Forbidden()
    {
        // arrange

        var teamId = new Guid("0f42c786-9d05-439d-4c0b-08dcc35177cb");
        var taskId = new Guid("63199241-decd-4cce-a30a-08dcc1d13e72");
        var client = _factory.CreateClient();
        var content = new CompleteTaskByTeamForm() { Duration = 10 };

        // act

        _authMock.Setup(m => m.Authorize(It.IsAny<TeamEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(false);
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        var result = await client.PutAsync($"api/team/{teamId}/task/{taskId}/complete", stringContent);
        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }
}