using Mailjet.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Todo.Application.Email.Interfaces;
using Todo.Application.Email.Models;
using Todo.Domain.Exceptions;
using Todo.Infrastructure.Email.EmailService;

namespace Todo.Infrastructure.Email.EmailProvider;

internal class MailJetProvider : EmailService.EmailSender
{
    private readonly IConfiguration _configuration;

    public MailJetProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task Send(EmailModel emailModel)
    {
        try
        {
            JArray jArray = new JArray();
            JArray attachments = new JArray();
            if (emailModel.Attachments != null && emailModel.Attachments.Count() > 0)
            {

                emailModel.Attachments.ToList().ForEach(attachment => attachments.Add(
                    new JObject
                    {
                            new JProperty("Content-Type", attachment.ContentType),
                            new JProperty("Filename", attachment.Name),
                            new JProperty("Content", Convert.ToBase64String(attachment.Data)),

                    }));
            }
            jArray.Add(new JObject
                {
                    new JProperty("FromEmail", _configuration["Email:SenderEmail"]),
                    new JProperty("FromName", _configuration["Email:SenderName"]),
                    new JProperty("Recipients", new JArray
                    {
                        new JObject
                        {
                            new JProperty("Email", emailModel.EmailAddress),
                            new JProperty("Name", emailModel.EmailAddress),


                        }
                    }),
                    new JProperty("Subject", emailModel.Subject),
                    new JProperty("Text-part", emailModel.Body),
                    new JProperty("Html-part", emailModel.Body),
                    new JProperty("Attachments", attachments),



                });

            var client = EmailSender.CreateMailJetClient(_configuration);
            var request = new MailjetRequest
            {
                Resource = Mailjet.Client.Resources.Send.Resource
            }.Property(Mailjet.Client.Resources.Send.Messages, jArray);
            var response = await client.PostAsync(request);
        }
        catch (Exception)
        {
            throw new ApiException("A error has happened");
        }
    }
}

