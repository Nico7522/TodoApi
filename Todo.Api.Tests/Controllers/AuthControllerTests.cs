using Todo.Api.Controllers;
using Xunit;
using FluentAssertions;
using System.Net.Http.Json;
using Todo.Application.Team.Dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;
using Todo.Api.Tests;
using Newtonsoft.Json;
using System.Text;
using Todo.Application.Team.Commands.CreateTeam;
using Todo.Application.Users.Commands.Register;
using Todo.Application.Users.Commands.Login;
using Azure;
using System.Web;

namespace Todo.Api.Controllers.Tests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
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
    public async void Register_ForValidRequest_Return200Ok()
    {
        // arrange

        var client = _factory.CreateClient();
        var content = new RegisterCommand()
        {
            PhoneNumber = "491410952",
            Email = "testcontrollerrr@gmail.com",
            FirstName = "Test",
            LastName = "Test",
            Password = "@Test12345",
            PasswordConfirm = "@Test12345",
            BirthDate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2020, 01, 01)

        };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        var result = await client.PostAsync("api/auth/register", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact()]
    public async void Register_ForInvalidRequestEmailAlreadyUsed_Return400Ok()
    {
        // arrange

        var client = _factory.CreateClient();
        var content = new RegisterCommand()
        {
            PhoneNumber = "491410952",
            Email = "testcontrollerr@gmail.com",
            FirstName = "Test",
            LastName = "Test",
            Password = "@Test12345",
            PasswordConfirm = "@Test12345",
            BirthDate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2020, 01, 01)

        };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        var result = await client.PostAsync("api/auth/register", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void Register_ForInvalidRequestInvalidFormBadEmail_Return400Ok()
    {
        // arrange

        var client = _factory.CreateClient();
        var content = new RegisterCommand()
        {
            PhoneNumber = "491410956",
            Email = "testcontrollerr@gmail",
            FirstName = "Test",
            LastName = "Test",
            Password = "@Test12345",
            PasswordConfirm = "@Test12345",
            BirthDate = new DateOnly(2000, 01, 01),
            HireDate = new DateOnly(2020, 01, 01)

        };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        var result = await client.PostAsync("api/auth/register", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void Register_ForInvalidRequestInvalidFormHireDateLowerThanBirthdate_Return400Ok()
    {
        // arrange

        var client = _factory.CreateClient();
        var content = new RegisterCommand()
        {
            PhoneNumber = "491410956",
            Email = "testcontrollerr@gmail.com",
            FirstName = "Test",
            LastName = "Test",
            Password = "@Test12345",
            PasswordConfirm = "@Test12345",
            BirthDate = new DateOnly(2020, 01, 01),
            HireDate = new DateOnly(2000, 01, 01)

        };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        var result = await client.PostAsync("api/auth/register", stringContent);

        // assert

        result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact()]
    public async void Login_ForValidRequest_Return200OkWithToken()
    {
        // arrange

        var client = _factory.CreateClient();
        var content = new LoginCommand() { Email = "nico.daddabbo7100@gmail.com", Password = "@Test12345" };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        var response = await client.PostAsync("api/auth/login", stringContent);
        var result = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        result.Should().NotBeNull();
    }

    [Fact()]
    public async void Login_ForInvalidRequestEmailNotFound_Return400BadRequest()
    {
        // arrange

        var client = _factory.CreateClient();
        var content = new LoginCommand() { Email = "nico.daddabbo710@gmail.com", Password = "@Test12345" };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        var response = await client.PostAsync("api/auth/login", stringContent);
        var result = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        result.Should().Be("\"Bad credentials\"");
    }


    [Fact()]
    public async void Login_ForInvalidRequestPasswordInvalid_Return400BadRequest()
    {
        // arrange

        var client = _factory.CreateClient();
        var content = new LoginCommand() { Email = "nico.daddabbo7100@gmail.com", Password = "@Test2345" };
        var contentJson = JsonConvert.SerializeObject(content);
        var stringContent = new StringContent(contentJson, Encoding.UTF8, "application/json");
        // act

        var response = await client.PostAsync("api/auth/login", stringContent);
        var result = await response.Content.ReadAsStringAsync();

        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        result.Should().Be("\"Bad credentials\"");
    }

    [Fact()]
    public async void ConfirmAccount_ForValidRequest_Return204NoContent()
    {
        // arrange

        var client = _factory.CreateClient();
        var userId = "4cd83e6b-923c-42c4-a92f-06d89e0a5630";
        var token = "CfDJ8KXQTLsQFzZLlK%2bF4p7lK3rAlURk1bDFwtgW8jS6V89i6d4vhGrbNH%2fpLc3kiwlNQCvbQDoUDid07Ih5KgXH4KgvuIrHkvwLZEjykKYrwc1N869EEOeA8CkpWI05Y2inlO61vGCwsq72Jj5iVmkBuLBHhX3DX5MJsQ%2bTjSxdfnAkx%2bf%2fHAeIahFjx8aaa0KmOV41wfZao2osYxU2NMXpASWWqyacFEmx1%2by97WC5vaIZE2256T5YUzZoabRHqqd0DQ%3d%3d";
        var encodedToken = HttpUtility.UrlEncode(token);
        // act

        var response = await client.PostAsync($"api/auth/confirmaccount?userId={userId}&token={encodedToken}", null);


        // assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact()]
    public async void ConfirmAccount_ForInvalidRequestUserNotFound_Return404NotFound()
    {
        // arrange

        var client = _factory.CreateClient();
        var userId = "4cd83e6b-923c-42c4-a92f-06d89e0a5630";
        var token = "CfDJ8KXQTLsQFzZLlK%2bF4p7lK3rAlURk1bDFwtgW8jS6V89i6d4vhGrbNH%2fpLc3kiwlNQCvbQDoUDid07Ih5KgXH4KgvuIrHkvwLZEjykKYrwc1N869EEOeA8CkpWI05Y2inlO61vGCwsq72Jj5iVmkBuLBHhX3DX5MJsQ%2bTjSxdfnAkx%2bf%2fHAeIahFjx8aaa0KmOV41wfZao2osYxU2NMXpASWWqyacFEmx1%2by97WC5vaIZE2256T5YUzZoabRHqqd0DQ%3d%3d";
        var encodedToken = HttpUtility.UrlEncode(token);
        // act

        var response = await client.PostAsync($"api/auth/confirmaccount?userId={userId}&token={encodedToken}", null);
        var result = await response.Content.ReadAsStringAsync();


        // assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        result.Should().Be("\"User not found\"");

    }

    [Fact()]
    public async void ConfirmAccount_ForInvalidRequestInvalidToken_Return400BadRequest()
    {
        // arrange

        var client = _factory.CreateClient();
        var userId = "6b72df6e-39cb-4e57-8ddc-1b4b298d7a41";
        var token = "CfDJ8KXQTLsQFzZLlK%2bF4p7lK3rAlURk1bDFwtgW8jS6V89i6d4vhGrbNH%2fpLc3kiwlNQCvbQDoUDid07Ih5KgXH4KgvuIrHkvwLZEjykKYrwc1N869EEOeA8CkpWI05Y2inlO61vGCwsq72Jj5iVmkBuLBHhX3DX5MJsQ%2bTjSxdfnAkx%2bf%2fHAeIahFjx8aaa0KmOV41wfZao2osYxU2NMXpASWWqyacFEmx1%2by97WC5vaIZE2256T5YUzZoabRHqqd0DQ%3d%3d";
        var encodedToken = HttpUtility.UrlEncode(token);
        // act

        var response = await client.PostAsync($"api/auth/confirmaccount?userId={userId}&token={encodedToken}", null);
        var result = await response.Content.ReadAsStringAsync();


        // assert

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        result.Should().Be("\"Email could not be confirmed\"");

    }
}