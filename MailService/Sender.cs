using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using MimeKit;

namespace MailService
{
    public class Sender : ISender
    {
        private readonly EmailConfiguration _emailConfig;

        public Sender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendEmailAsync(Message message)
        {
            var mailMessage = CreateEmailMessage(message);
            await SendAsync(mailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            MimeMessage emailMessage = new();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.UserName, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            BodyBuilder bodyBuilder = new() { HtmlBody = $"<h2 style='color:black;'>{message.Content}</h2>" };

            if (message.Attachments.Any())
            {
                foreach (IFormFile attachment in message.Attachments)
                {
                    byte[] fileBytes;
                    using (MemoryStream ms = new())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }

            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }
        
        private async Task SendAsync(MimeMessage mailMessage)
        {
            using SmtpClient client = new();
            try
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                await client.SendAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}