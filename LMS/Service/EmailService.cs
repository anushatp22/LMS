using LMS.DTOs.Common;
using LMS.Interface;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LMS.Service
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }
        public async Task<bool> SendTempPasswordAsync(string email, string password, string companyName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("LMS System", _mailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "Your Temporary Login Credentials";

                message.Body = new TextPart("plain")
                {
                    Text = $"Welcome to {companyName},\n\nYour temporary login email is: {email} and password is: {password}\n" +
                           $"Please change it after your first login.\n\nThanks,\n{companyName}"
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> SendLeaveApprovalMail(string Email)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("LMS System", _mailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", Email));
                message.Subject = "Leave Application";
                message.Body = new TextPart("plain")
                {
                    Text = $"Your Leave Request has been approved. You can check in LMS"
                };
                using var client = new SmtpClient();
                await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
