using AutoMapper.Configuration.Annotations;
using MediatR;
using System.Text.Json.Serialization;
using Todo.Domain.Entities;
using Todo.Domain.Enums;

namespace Todo.Application.Task.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest
{
    [JsonIgnore]
    public Guid? Id { get; set; } 
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Priority Priority { get; set; }
}
