namespace Todo.Application.Email.Interfaces;

internal interface IEmailSender
{
    Task SendEmail(string address, string subject, string body, List<EmailAttachment> emailAttachment = null);
    Task SendEmail(EmailModel emailModel);
}
