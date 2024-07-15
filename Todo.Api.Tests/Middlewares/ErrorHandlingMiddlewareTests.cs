using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;
using System.Text.Json;
using Todo.Domain.Exceptions;
using Xunit;


namespace Todo.Api.Middlewares.Tests;

public class ErrorHandlingMiddlewareTests
{
    [Fact()]
    public async void InvokeAsync_WhenNoExceptionThrown_ShouldCallNextDelegate()
    {
        // arrange

        var middleware = new ErrorHandlingMiddleware();
        var context = new DefaultHttpContext();
        var nextDelegateMock = new Mock<RequestDelegate>();

        // act

        await middleware.InvokeAsync(context, nextDelegateMock.Object );

        // assert

        nextDelegateMock.Verify(next => next.Invoke(context), Times.Once);
    }

    [Fact()]
    public async void InvokeAsync_WhenNotFoundExceptionThrown_ShouldSetStatusCodeTo404AndWriteExceptionMessage()
    {
        // arrange

        var middleware = new ErrorHandlingMiddleware();
        var context = new DefaultHttpContext();
        var notFoundException = new NotFoundException("Task not found");

        // act

        await middleware.InvokeAsync(context, _ => throw notFoundException);

        // assert

        context.Response.StatusCode.Should().Be(404);
 

    }

    [Fact()]
    public async void InvokeAsync_WhenBadRequestExceptionThrown_ShouldSetStatusCodeTo400AndWriteExceptionMessage()
    {
        // arrange

        var middleware = new ErrorHandlingMiddleware();
        var context = new DefaultHttpContext();
        var badRequestException = new BadRequestException("Error");

        // act

        await middleware.InvokeAsync(context, _ => throw badRequestException);

        // assert

        context.Response.StatusCode.Should().Be(400);


    }

    [Fact()]
    public async void InvokeAsync_WhenForbidExceptionThrown_ShouldSetStatusCodeTo403AndWriteExceptionMessage()
    {
        // arrange

        var middleware = new ErrorHandlingMiddleware();
        var context = new DefaultHttpContext();
        var forbidException = new ForbidException("Your not authorized");

        // act

        await middleware.InvokeAsync(context, _ => throw forbidException);

        // assert

        context.Response.StatusCode.Should().Be(403);
    }

    [Fact()]
    public async void InvokeAsync_WhenValidationExceptionThrown_ShouldSetStatusCodeTo400AndWriteExceptionMessage()
    {
        // arrange

        var middleware = new ErrorHandlingMiddleware();
        var context = new DefaultHttpContext();
        var validationException = new ValidationException("Error");

        // act

        await middleware.InvokeAsync(context, _ => throw validationException);

        // assert

        context.Response.StatusCode.Should().Be(400);
    }

    [Fact()]
    public async void InvokeAsync_WhenExceptionThrown_ShouldSetStatusCodeTo500AndWriteExceptionMessage()
    {
        // arrange

        var middleware = new ErrorHandlingMiddleware();
        var context = new DefaultHttpContext();
        var exception = new Exception();

        // act

        await middleware.InvokeAsync(context, _ => throw exception);

        // assert

        context.Response.StatusCode.Should().Be(500);


    }

    private async Task<string> ReadResponseBodyAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(response.Body, Encoding.UTF8);
        var responseBody = await reader.ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return responseBody;
    }
}