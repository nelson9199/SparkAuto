using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Email
{
    public class EmailSender : IEmailSender
    {
        public EmailOptions Options { get; set; }

        //La interfaz IOptions<T> me sirve para devolbet instacias configuradas del tipo que le pase como genérico,
        //en este caso vou a recibir instancias configuradas de tipo EmailOptions y esta configuracion la voy a traer
        //desde la clase Startup mediante dependency injection
        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            Options = emailOptions.Value;
        }

        public async Task Execute(string email, string subject, string htmlMessage)
        {
            var apiKey = Options.SendGridKey;
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("nelsonmarro99@outlook.com", "Spark Auto");
            var to = new EmailAddress(email);
            var Subject = subject;
            var plainTextContent = htmlMessage;
            var htmlContext = htmlMessage;

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                Subject,
                plainTextContent,
                htmlContext
                );
            var response = await client.SendEmailAsync(msg);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await Execute(email, subject, htmlMessage);
        }
    }
}
