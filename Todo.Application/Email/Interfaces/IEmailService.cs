namespace Todo.Application.Email.Interfaces;
public interface IEmailService
{
    System.Threading.Tasks.Task SendEmailAsync(string toEmail, string subject, string body, bool osBodyHTML);
}
