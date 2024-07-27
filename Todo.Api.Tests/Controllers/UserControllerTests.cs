using Xunit;
using Todo.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;
using Todo.Api.Tests;
using FluentAssertions;
using System.Net.Http.Json;
using Todo.Application.Team.Dto;
using Todo.Application.Users.Dto;

namespace Todo.Api.Controllers.Tests
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UserControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    //services.Replace(ServiceDescriptor.Scoped(typeof(IAuthorization<TeamEntity>), _ => _authMock.Object));
                });
            });
        }
        [Fact()]
        public async void GetUserBy_ForValidRequest_Return200OkWithUser()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
            var result = await client.GetAsync($"api/user/{userId}");
            var content = await client.GetFromJsonAsync<UserDto>($"api/user/{userId}");

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNull();
        }

        [Fact()]
        public async void GetUserBy_ForInvalidRequest_Return404NotFound()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5edb6";
            var result = await client.GetAsync($"api/user/{userId}");

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async void AssignTaskByUser_forValidRequest_Return200Ok()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var taskId = new Guid("9382f33f-acf8-431d-55b3-08dca4d8e649");
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
            var result = await client.PostAsync($"api/user/{userId}/task/{taskId}", null);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact()]
        public async void AssignTaskByUser_forInvalidRequestTaskNotFound_Return404NotFound()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var taskId = new Guid("9382f33f-acf9-431d-55b3-08dca4d8e649");
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
            var result = await client.PostAsync($"api/user/{userId}/task/{taskId}", null);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async void AssignTaskByUser_forInvalidRequestUserNotFound_Return404NotFound()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var taskId = new Guid("9382f33f-acf8-431d-55b3-08dca4d8e649");
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda7";
            var result = await client.PostAsync($"api/user/{userId}/task/{taskId}", null);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async void AssignTaskByUser_forInvalidRequestTaskAlreadyAssignedToUser_Return400BadRequest()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var taskId = new Guid("9382f33f-acf8-431d-55b3-08dca4d8e649");
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
            var result = await client.PostAsync($"api/user/{userId}/task/{taskId}", null);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact()]
        public async void AssignTaskByUser_forInvalidRequestTaskAlreadyAssignedToAnotherUserOrATeam_Return400BadRequest()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var taskId = new Guid("9f150232-005e-43b2-3419-08dc8c7b4fcd");
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
            var result = await client.PostAsync($"api/user/{userId}/task/{taskId}", null);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}