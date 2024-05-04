using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;
//using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using PostmarkDotNet.Model;
using PostmarkDotNet;
using System.Net.Mail;
using System.Net;

namespace MelodyCircle.Services
{
    /* Eduardo Andrade */
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration configuration;
        private readonly ILogger logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            using (var client = new SmtpClient())
            {
                client.Host = "smtp.gmail.com";
                client.Port = 587;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("melodycirclehelp@gmail.com", "rbrb cyic lzgw spfg");
                using (var mail = new MailMessage(
                    from: new MailAddress("melodycirclehelp@gmail.com", "Support Melodycircle"),
                    to: new MailAddress(toEmail, "THEIR NAME")
                ))
                {

                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    mail.Body = message;


                    await client.SendMailAsync(mail);
                }
            }
        }
    }
}
