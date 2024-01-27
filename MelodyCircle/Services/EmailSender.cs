using Microsoft.AspNetCore.Identity;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MelodyCircle.Services
{
    public class EmailSender : IEmailSender
    {
        //// Our private configuration variables
        //private string host;
        //private int port;
        //private bool enableSSL;
        //private string userName;
        //private string password;

        //// Get our parameterized configuration
        //public EmailSender(string host, int port, bool enableSSL, string userName, string password)
        //{
        //    this.host = host;
        //    this.port = port;
        //    this.enableSSL = enableSSL;
        //    this.userName = userName;
        //    this.password = password;
        //}

        //public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    var message = new MimeMessage();
        //    message.From.Add(new MailboxAddress("MiniESTS", "pv-2122@outlook.com"));
        //    message.To.Add(new MailboxAddress(userName, email));
        //    message.Subject = subject;
        //    message.Body = new TextPart(TextFormat.Html)
        //    {
        //        Text = htmlMessage
        //    };

        //    using (var client = new SmtpClient())
        //    {
        //        // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
        //        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

        //        await client.ConnectAsync(host, port, enableSSL);

        //        // Note: only needed if the SMTP server requires authentication
        //        await client.AuthenticateAsync(userName, password);

        //        await client.SendAsync(message);
        //        await client.DisconnectAsync(true);
        //    }
        //}

        private readonly IConfiguration configuration;
        private readonly ILogger logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            string sendGridApiKey = configuration["EmailSender:SendGridAPIKey"];
            if (string.IsNullOrEmpty(sendGridApiKey))
            {
                throw new Exception("The 'SendGridApiKey' is not configured");
            }

            var client = new SendGridClient(sendGridApiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("melodycirclemail@gmail.com", "MelodyCircle"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail));

            var response = await client.SendEmailAsync(msg);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Email queued successfully");
            }
            else
            {
                logger.LogError("Failed to send email");
                // Adding more information related to the failed email could be helpful in debugging failure,
                // but be careful about logging PII, as it increases the chance of leaking PII
            }
        }
    }
}
