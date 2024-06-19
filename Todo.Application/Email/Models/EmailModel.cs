namespace Todo.Application.Email.Models;

public class EmailModel
{
    public string EmailAddress { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public IEnumerable<EmailAttachment>? Attachments { get; set; }
}
