using Todo.Domain.Enums;

namespace Todo.Api.Forms.UpdateTaskByTeamForm;

public class UpdateTaskByTeamForm
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public Priority Priority { get; set; }
}
