using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
namespace Todo.Application.ValidationResultExtensions;

public static class ValidationResultExtensions
{
    public static IActionResult ToValidationProblem(this ValidationResult result)
    {
        return new BadRequestObjectResult(new ValidationProblemDetails(result.ToDictionary()));
    }
}
