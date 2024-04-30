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

        //public async Task SendEmailAsync(string toEmail, string subject, string message)
        //{
        //    string sendGridApiKey = configuration["EmailSender:SendGridAPIKey"];
        //    if (string.IsNullOrEmpty(sendGridApiKey))
        //    {
        //        throw new Exception("The 'SendGridApiKey' is not configured");
        //    }

        //    var client = new SendGridClient(sendGridApiKey);
        //    var msg = new SendGridMessage()
        //    {
        //        From = new EmailAddress("melodycirclemail@gmail.com", "MelodyCircle"),
        //        Subject = subject,
        //        PlainTextContent = message,
        //        HtmlContent = message
        //    };
        //    msg.AddTo(new EmailAddress(toEmail));

        //    var response = await client.SendEmailAsync(msg);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        logger.LogInformation("Email queued successfully");
        //    }
        //    else
        //    {
        //        logger.LogError("Failed to send email");
        //    }
        //}

        //public async Task SendEmailAsync(string toEmail, string subject, string message)
        //{
        //    var msg = new PostmarkMessage()
        //    {
        //        To = toEmail,
        //        From = "202000906@estudantes.ips.pt",
        //        TrackOpens = true,
        //        Subject = subject,
        //        TextBody = message,
        //        //HtmlBody = "HTML goes here",
        //        //Tag = "New Year's Email Campaign",
        //    };

        //    var client = new PostmarkClient("");
        //    var sendResult = await client.SendMessageAsync(msg);

        //    if (sendResult.Status == PostmarkStatus.Success) { logger.LogInformation("Email queued successfully"); }
        //    else { logger.LogError("Failed to send email"); }
        //}

        //public async Task SendEmailAsync(string toEmail, string subject, string message)
        //{
        //    var email = new MimeMessage();

        //    email.From.Add(new MailboxAddress("Support MelodyCircle", "melodycirclehelp@gmail.com"));
        //    email.To.Add(new MailboxAddress("Receiver Name", toEmail));

        //    email.Subject = subject;
        //    email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        //    {
        //        Text = message
        //    };

        //    using (var smtp = new SmtpClient())
        //    {
        //        smtp.Connect("smtp.gmail.com", 587, false);

        //        // Note: only needed if the SMTP server requires authentication
        //        smtp.Authenticate("melodycirclehelp@gmail.com", "#helpsupport");

        //        smtp.Send(email);
        //        smtp.Disconnect(true);
        //    }
        //}

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
