using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Todo.Application.Task.Commands.UpdateTask;



namespace Todo.Application.Extensions;
public static class ServicesCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()).AddFluentValidationAutoValidation();
        services.AddScoped<IValidator<UpdateTaskCommand>, UpdateTaskCommandValidator>();

        services.AddHttpContextAccessor();
    }
}
