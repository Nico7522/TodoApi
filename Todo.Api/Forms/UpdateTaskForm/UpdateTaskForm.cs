using Todo.Domain.Enums;
namespace Todo.Api.Forms.UpdateTaskForm;

public class UpdateTaskForm
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Priority Priority { get; set; }
}
