using Microsoft.Extensions.Configuration;
using Personal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Personal.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private IConfiguration _configuration { get; set; }

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("admin@personal.com");
            mailMessage.To.Add(email);
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;
            _SendEmail(mailMessage);

            return Task.CompletedTask;
        }

        public Task SendEmailAsync(string toAddress, string fromAddress, string subject, string message)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromAddress);
            mailMessage.To.Add(toAddress);
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;
            _SendEmail(mailMessage);
            return Task.CompletedTask;
        }

        private void _SendEmail(MailMessage message)
        {
            string host = _configuration.GetValue<string>(ConfigUtils.Path.SMTP_HOST);
            int port = _configuration.GetValue<int>(ConfigUtils.Path.SMTP_PORT);
            SmtpClient client = new SmtpClient(host,port);
            client.EnableSsl = false;
            client.UseDefaultCredentials = false;

            string username = _configuration.GetValue<string>(ConfigUtils.Path.SMTP_USERNAME);
            string password = _configuration.GetValue<string>(ConfigUtils.Path.SMTP_PASSWORD);
            client.Credentials = new NetworkCredential(username, password);
            client.Send(message);
            client.Dispose();
        }
    }
}
