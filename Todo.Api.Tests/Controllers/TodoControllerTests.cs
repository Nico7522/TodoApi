using Todo.Api.Controllers;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using Todo.Application.Task.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using Todo.Domain.Repositories;
using MediatR;
using Todo.Infrastructure.Services;
using System.Text;
using Todo.Api.Forms.CompleteTaskForm;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;
using Todo.Api.Tests;
using Moq;
using Todo.Domain.AuthorizationInterfaces;
using Todo.Domain.Entities;
using Todo.Infrastructure.Authorization.Service;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Todo.Application.Task.Commands.UpdateTask;
using Todo.Application.Task.Commands.DeleteTask;
using Todo.Application.Task.Commands.CreateTask;

namespace Todo.Api.Controllers.Tests;

public class TodoControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IAuthorization<TodoEntity>> _authMock = new();
    public TodoControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                services.Replace(ServiceDescriptor.Scoped(typeof(IAuthorization<TodoEntity>), _ => _authMock.Object));
            });
        });
    }
    [Fact()]
    public async void GetAll_ForValidRequest_Returns200Ok()
    {
        // arrange

        var client = _factory.CreateClient();

        // act
        var result = await client.GetAsync("/api/todo/active");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact()]
    public async void GetTaskById_ForValidRequest_Return200OkWithTask()
    {
        // arrange
        var id = "b3232cb2-1b59-4b19-3418-08dc8c7b4fcd";
        var client = _factory.CreateClient();

        // act
        var httpStatusCodeResult = await client.GetAsync($"/api/todo/{id}");
        var result = await client.GetFromJsonAsync<TaskDto>($"/api/todo/{id}");

        // assert
        httpStatusCodeResult.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Id.Should().Be(new Guid(id));
        result.Priority.Should().Be(Domain.Enums.Priority.Low);

    }

    [Fact()]
    public async void GetTaskById_ForInvalidRequest_Return404NotFound()
    {
        // arrange
        var id = Guid.NewGuid();
        var client = _factory.CreateClient();

        // act
        var result = await client.GetAsync($"/api/todo/{id}");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void CompleteTask_ForValidRequest_Return200Ok()
    {
        // arrange
        var taskId = new Guid("8f6c2609-a681-4df8-341a-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new CompleteTaskForm
        {
            Duration = 30,

        };
        _authMock.Setup(m => m.Authorize(It.IsAny<TodoEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(true);
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act
        var result = await client.PutAsync($"/api/todo/{taskId}/complete", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }


    [Fact()]
    public async void CompleteTask_ForInvalidRequest_Return404NotFound()
    {
        // arrange
        var id = new Guid("9f150232-015e-43b2-3419-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new CompleteTaskForm
        {
            Duration = 30,

        };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act
        var result = await client.PutAsync($"/api/todo/{id}/complete", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void CompleteTask_ForInvalidRequest_Return403Borbiden()
    {
        // arrange
        var id = new Guid("2eea5ee7-482d-4789-341b-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new CompleteTaskForm
        {
            Duration = 30,

        };
        _authMock.Setup(m => m.Authorize(It.IsAny<TodoEntity>(), Domain.Enums.RessourceOperation.Update)).Returns(false);
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act
        var result = await client.PutAsync($"/api/todo/{id}/complete", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
    }

    [Fact()]
    public async void UpdateTask_ForValidCommand_Return200Ok()
    {
        // arrange
        var id = new Guid("2eea5ee7-482d-4789-341b-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new UpdateTaskCommand(id, "test", "test", Domain.Enums.Priority.High);

        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act
        var result = await client.PutAsync($"/api/todo/{id}", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }
    [Fact()]
    public async void UpdateTask_ForInvalidCommand_Return404NotFound()
    {
        // arrange
        var id = new Guid("2eea5ee7-482d-4789-341b-08dc8c7b4fcc");
        var client = _factory.CreateClient();
        var content = new UpdateTaskCommand(id, "test", "test", Domain.Enums.Priority.High);

        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act
        var result = await client.PutAsync($"/api/todo/{id}", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
    [Fact()]
    public async void UpdateTask_ForInvalidCommand_Return400BadRequest()
    {
        // arrange
        var id = new Guid("2eea5ee7-482d-4789-341b-08dc8c7b4fcd");
        var client = _factory.CreateClient();
        var content = new UpdateTaskCommand(id, "", "test", Domain.Enums.Priority.High);

        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act
        var result = await client.PutAsync($"/api/todo/{id}", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void DeleteTask_ForValidCommand_Return200Ok()
    {
        // arrange
        var id = new Guid("2eea5ee7-482d-4789-341b-08dc8c7b4fcd");
        var client = _factory.CreateClient();

        // act
        var result = await client.DeleteAsync($"/api/todo/{id}");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }


    [Fact()]
    public async void DeleteTask_ForInvalidCommand_Return404NotFound()
    {
        // arrange
        var id = new Guid("2eea5ee7-482d-4789-341b-08dc8c7b4fdd");
        var client = _factory.CreateClient();

        // act
        var result = await client.DeleteAsync($"/api/todo/{id}");

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact()]
    public async void CreateTask_ForValidCommand_Return201CreatedWithTask()
    {
        // arrange
        var client = _factory.CreateClient();
        var content = new CreateTaskCommand() { Description = "test", Priority = Domain.Enums.Priority.Low, Title = "test" };

        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act
        var result = await client.PostAsync($"/api/todo", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact()]
    public async void CreateTask_ForInvalidCommand_Return400badRequest()
    {
        // arrange
        var client = _factory.CreateClient();
        var content = new CreateTaskCommand() { Description = "", Priority = Domain.Enums.Priority.Low, Title = "test" };

        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act
        var result = await client.PostAsync($"/api/todo", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void UnassignTask_ForValidCommand_Return204NoContent()
    {
        // arrange
        var id = new Guid("b3232cb2-1b59-4b19-3418-08dc8c7b4fcd");
        var client = _factory.CreateClient();
 
        // act
        var result = await client.PutAsync($"/api/todo/{id}/unassign", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact()]
    public async void UnassignTask_ForValidCommand_Return404NotFound()
    {
        // arrange
        var id = new Guid("b3232cb2-1b59-4b19-3418-08dc8c7b4fdd");
        var client = _factory.CreateClient();

        // act
        var result = await client.PutAsync($"/api/todo/{id}/unassign", null);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}