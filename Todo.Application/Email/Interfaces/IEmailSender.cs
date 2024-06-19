using Todo.Application.Email.Models;
namespace Todo.Application.Email.Interfaces;

public interface IEmailSender
{
    System.Threading.Tasks.Task SendEmail(string address, string subject, string body, List<EmailAttachment> emailAttachment = null);
    System.Threading.Tasks.Task SendEmail(EmailModel emailModel);
}
