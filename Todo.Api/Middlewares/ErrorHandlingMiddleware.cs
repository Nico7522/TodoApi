using FluentValidation;
using System.Net.Mime;
using System.Text.Json;
using Todo.Domain.Exceptions;

namespace Todo.Api.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (ApiErrorException ex)
        {

            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.Message));
        }
        catch (ValidationException ex)
        {
            //var response = new {Errors = ex.Errors, StatusCode = ex.StatusCode};
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.Errors.Select(e => e.ErrorMessage)));
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Error");
        }
    }
}
