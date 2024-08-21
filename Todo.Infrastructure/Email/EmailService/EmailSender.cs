using Mailjet.Client;
using Microsoft.Extensions.Configuration;

using Todo.Application.Email.Models;
using IEmailSender = Todo.Application.Email.Interfaces.IEmailSender;

namespace Todo.Infrastructure.Email.EmailService
{
    public abstract class EmailSender : IEmailSender
    {
        public static MailjetClient CreateMailJetClient(IConfiguration configuration)
        {
            return new MailjetClient(configuration["Email:ApiKey"], configuration["Email:ApiSecret"]);
        }

        protected abstract Task Send(EmailModel emailModel);
        public async Task SendEmail(string address, string subject, string body, List<EmailAttachment> emailAttachment = null)
        {
            await Send(new EmailModel
            {
                Attachments = emailAttachment,
                Subject = subject,
                Body = body,
                EmailAddress = address

            });
        }

        public async Task SendEmail(EmailModel emailModel)
        {
            await Send(emailModel);
        }
    }
}
