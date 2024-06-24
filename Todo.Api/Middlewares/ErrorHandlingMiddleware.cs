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
        catch (NotFoundException ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.Message));
        }
        catch (BadRequestException ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.Message));
        }
        catch (ValidationException ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.Errors.Select(e => e.ErrorMessage)));
        }
        catch (ApiException ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.Message));
        }

        catch (ForbidException ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = 403;
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.Message));
        }
        catch (Exception ex)
        {
            string errorMessage = "";
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = 500;
            if(ex.InnerException != null && ex.InnerException.Message.Contains("Account")) errorMessage = ex.InnerException.Message;
            else errorMessage = "Server error, please try later";
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorMessage));
        }
    }
}
