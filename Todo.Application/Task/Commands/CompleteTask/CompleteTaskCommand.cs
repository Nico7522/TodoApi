using AutoMapper.Configuration.Annotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Todo.Application.Task.Commands.CompleteTask;

public class CompleteTaskCommand : IRequest<bool>
{
    public Guid TaskId { get; init; }
    public int Duration { get; init; }

    public CompleteTaskCommand(Guid taskId, int duration)
    {
        TaskId = taskId;
        Duration = duration;
    }

}
