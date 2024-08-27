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
using Todo.Application.Users.Commands.AssignRole;
using Newtonsoft.Json;
using Todo.Api.Forms.AssignRoleForm;
using Moq;
using Todo.Application.Users;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Todo.Domain.Constants;
using Todo.Application.Users.Commands.ResetPassword;
using Mailjet.Client.Resources;
using Todo.Api.Forms.ResetPasswordConfirmForm;
using FluentValidation;
using Todo.Application.Users.Commands.ResetPasswordConfirm;
using System.Web;

namespace Todo.Api.Controllers.Tests
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IUserContext> _userContextMock = new();
        private readonly Mock<IValidator<ResetPasswordConfirmCommand>> _validatorMock = new();

        public UserControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.Replace(ServiceDescriptor.Scoped(typeof(IUserContext), _ => _userContextMock.Object));
                    services.Replace(ServiceDescriptor.Scoped(typeof(IValidator<ResetPasswordConfirmCommand>), _ => _validatorMock.Object));

                });
            });
        }
        [Fact()]
        public async void GetUserById_ForValidRequest_Return200OkWithUser()
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
        public async void GetUserById_ForInvalidRequest_Return404NotFound()
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
        public async void GetTeamByUser_ForValidRequest_Return200OkWithUserTeam()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var userId = "f8ff6f82-f931-4195-ac6c-5d238df5378d";
            var result = await client.GetAsync($"api/user/{userId}/team");
            var content = await client.GetFromJsonAsync<TeamDto>($"api/user/{userId}/team");

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNull();
        }

        [Fact()]
        public async void GetTeamByUser_ForValidRequest_Return204NoContent()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var userId = "0e03a811-4a2c-498a-b8c8-ef7ce4423869";
            var result = await client.GetAsync($"api/user/{userId}/team");

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact()]
        public async void GetTeamByUser_ForInvalidRequestUserNotFound_Return404NotFound()
        {
            // arrange

            var client = _factory.CreateClient();

            // act
            var userId = "userid";
            var result = await client.GetAsync($"api/user/{userId}/team");

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async void AssignTaskByUser_ForValidRequest_Return200Ok()
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
        public async void AssignTaskByUser_ForInvalidRequestTaskNotFound_Return404NotFound()
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
        public async void AssignTaskByUser_ForInvalidRequestUserNotFound_Return404NotFound()
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
        public async void AssignTaskByUser_ForInvalidRequestTaskAlreadyAssignedToUser_Return400BadRequest()
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
        public async void AssignTaskByUser_ForInvalidRequestTaskAlreadyAssignedToAnotherUserOrATeam_Return400BadRequest()
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

        [Fact()]
        public async void AssignRole_ForValidRequest_Return204NoContent()
        {
            // arrange

            var client = _factory.CreateClient();
            var content = new AssignRoleForm() { Role = UserRole.Leader };
            var contentJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
            var currentUser = new CurrentUser("6b475eb4-70c9-440e-94c7-31d415c8c564", "", UserRole.SuperAdmin);

            // act
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = await client.PutAsync($"api/user/{userId}/assignrole", stringContent);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact()]
        public async void AssignRole_ForInvalidRequestBadRole_Return400NoBadRequest()
        {
            // arrange

            var client = _factory.CreateClient();
            var content = new AssignRoleForm() { Role = "Leade" };
            var contentJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
            var currentUser = new CurrentUser("6b475eb4-70c9-440e-94c7-31d415c8c564", "", UserRole.SuperAdmin);

            // act
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = await client.PutAsync($"api/user/{userId}/assignrole", stringContent);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact()]
        public async void AssignRole_ForInvalidRequestActionCantBeAchieved_Return400NoBadRequest()
        {
            // arrange

            var client = _factory.CreateClient();
            var content = new AssignRoleForm() { Role = UserRole.SuperAdmin };
            var contentJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
            var currentUser = new CurrentUser("6b475eb4-70c9-440e-94c7-31d415c8c564", "", UserRole.Admin);

            // act
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = await client.PutAsync($"api/user/{userId}/assignrole", stringContent);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact()]
        public async void AssignRole_ForInvalidRequestUserNotFound_Return404NotFound()
        {
            // arrange

            var client = _factory.CreateClient();
            var content = new AssignRoleForm() { Role = UserRole.Leader };
            var contentJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
            var userId = "47c03a1a-9838-4484-8126-c1d02f90812b";
            var currentUser = new CurrentUser("6b475eb4-70c9-440e-94c7-31d415c8c564", "", UserRole.SuperAdmin);

            // act
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = await client.PutAsync($"api/user/{userId}/assignrole", stringContent);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async void UnassignRole_ForValidRequest_Return204NoContent()
        {
            // arrange

            var client = _factory.CreateClient();
            var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
            var currentUser = new CurrentUser("6b475eb4-70c9-440e-94c7-31d415c8c564", "", UserRole.SuperAdmin);

            // act
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = await client.PutAsync($"api/user/{userId}/unassignrole", null);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact()]
        public async void UnassignRole_ForInvalidRequest_Return404NotFound()
        {
            // arrange

            var client = _factory.CreateClient();
            var userId = "47c03a1a-9838-4484-8126-c1d02f90812b";
            var currentUser = new CurrentUser("6b475eb4-70c9-440e-94c7-31d415c8c564", "", UserRole.SuperAdmin);

            // act
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = await client.PutAsync($"api/user/{userId}/unassignrole", null);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async void UnassignRole_ForInvalidRequestUserHasNorole_Return400BadRequest()
        {
            // arrange

            var client = _factory.CreateClient();
            var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";
            var currentUser = new CurrentUser("6b475eb4-70c9-440e-94c7-31d415c8c564", "", UserRole.SuperAdmin);

            // act
            _userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);
            var result = await client.PutAsync($"api/user/{userId}/unassignrole", null);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact()]
        public async void ResetPassword_ForValidRequest_Return204NoContent()
        {
            // arrange

            var client = _factory.CreateClient();
            var content = new ResetPasswordCommand("");
            var contentJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");

            // act
            var result = await client.PostAsync($"api/user/resetpassword", stringContent);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact()]
        public async void ResetPassword_ForInvalidRequest_Return404NotFound()
        {
            // arrange

            var client = _factory.CreateClient();
            var content = new ResetPasswordCommand("zzz@gmail.com");
            var contentJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");

            // act
            var result = await client.PostAsync($"api/user/resetpassword", stringContent);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }


        [Fact()]
        public async void ResetPasswordConfirm_ForValidRequest_Return204NoContent()
        {
            // arrange

            var client = _factory.CreateClient();
            var content = new ResetPasswordConfirmForm() { Password = "@!Test12345678", PasswordConfirm = "@!Test12345678" };
            string encodedToken =HttpUtility.UrlEncode("CfDJ8KXQTLsQFzZLlK%2bF4p7lK3rtBRYc5%2bBvyjpmWDncz5MuX%2bVpmJAHrbEtojd6azds5gqsRXtAkjAR4AwyNB0mYNpdECw7jZdiy%2blwQi3bzru05Zgsolm9RTxSdcoZYRXLSzgXCUOIkftZKjAMTi5forfagD6BTejIC0gkRuK95Xg7EyWdNR4V%2bMCWPWeYqHFAYFqr4imZe13qf5ZF%2fSDo3a8kudtxi8fSR4M1RwFRQCxo");
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
            var contentJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");

            // act
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ResetPasswordConfirmCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await client.PostAsync($"api/user/{userId}/{encodedToken}/resetpasswordconfirm", stringContent);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact()]
        public async void ResetPasswordConfirm_ForInvalidRequest_Return404NotFound()
        {
            // arrange

            var client = _factory.CreateClient();
            var content = new ResetPasswordConfirmForm() { Password = "@!Test12345678", PasswordConfirm = "@!Test12345678" };
            string encodedToken = HttpUtility.UrlEncode("CfDJ8KXQTLsQFzZLlK%2bF4p7lK3rtBRYc5%2bBvyjpmWDncz5MuX%2bVpmJAHrbEtojd6azds5gqsRXtAkjAR4AwyNB0mYNpdECw7jZdiy%2blwQi3bzru05Zgsolm9RTxSdcoZYRXLSzgXCUOIkftZKjAMTi5forfagD6BTejIC0gkRuK95Xg7EyWdNR4V%2bMCWPWeYqHFAYFqr4imZe13qf5ZF%2fSDo3a8kudtxi8fSR4M1RwFRQCxo");
            var userId = "a4c3d048-252e-450a-be51-9b2edbf5eda6";
            var contentJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");

            // act
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ResetPasswordConfirmCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await client.PostAsync($"api/user/{userId}/{encodedToken}/resetpasswordconfirm", stringContent);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async void ResetPasswordConfirm_ForInvalidRequestBadToken_Return400BadRequest()
        {
            // arrange

            var client = _factory.CreateClient();
            var content = new ResetPasswordConfirmForm() { Password = "@!Test12345678", PasswordConfirm = "@!Test12345678" };
            string encodedToken = HttpUtility.UrlEncode("BadTokenCfDJ8KXQTLsQFzZLlK%2bF4p7lK3ppdEJfHc5PCqJxcwdi2dY8beISHlFfncNFHw%2boLwlXh0hHY3wOgwmr33qgnOFbQzfUE2LRzSGjrWEeFFF%2fUwduIgpAKBqOpi6EcC8oWPNzK3BbOz5gquF0ijMzLB8%2fT0OgxLF%2fOwMfQzj673Ppb%2brIy12UgS3iNKDvoHuO%2fXKI8bMEVv4i1XC%2f27WB1KD9GFYBQ9i6fzbvRam%2bQ0nQPOv%2f");
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";
            var contentJson = JsonConvert.SerializeObject(content);
            var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");

            // act
            _validatorMock.Setup(v => v.ValidateAsync(It.IsAny<ResetPasswordConfirmCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());

            var result = await client.PostAsync($"api/user/{userId}/{encodedToken}/resetpasswordconfirm", stringContent);

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact()]
        public async void Delete_ForValidRequest_Return204NoContent()
        {
            // arrange

            var client = _factory.CreateClient();
            var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";

            // act
            var result = await client.DeleteAsync($"api/user/{userId}");

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact()]
        public async void Delete_ForInvalidRequest_Return404NotFound()
        {
            // arrange

            var client = _factory.CreateClient();
            var userId = "47c03a1a-9838-4484-8126-c1d02f90812a";

            // act
            var result = await client.DeleteAsync($"api/user/{userId}");

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact()]
        public async void GetTasksNumberByUser_ForValidRequest_Return200OkWithTaskNumber()
        {

            // arrange

            var client = _factory.CreateClient();
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda6";

            // act
            var result = await client.GetAsync($"api/user/{userId}/tasksnumber");
            var content = await client.GetFromJsonAsync<int>($"api/user/{userId}/tasksnumber");

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().Be(4);

        }


        [Fact()]
        public async void GetTasksNumberByUser_ForInvalidRequest_Return404NotFound()
        {

            // arrange

            var client = _factory.CreateClient();
            var userId = "e4c3d048-252e-450a-be51-9b2edbf5eda7";

            // act
            var result = await client.GetAsync($"api/user/{userId}/tasksnumber");

            // assert

            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

    }
}