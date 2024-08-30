namespace Todo.Infrastructure.ChatHub.Models;
 public class UserStatus
{
    public string Id { get; set; } = default!;
    public bool IsOnline { get; set; }
    public bool IsPresent { get; set; }
}
