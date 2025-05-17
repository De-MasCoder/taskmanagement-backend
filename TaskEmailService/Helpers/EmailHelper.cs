using MailKit.Net.Smtp;
using MimeKit;

namespace TaskEmailService.Helpers
{
    public static class EmailHelper
    {
        public static async Task SendEmailAsync(string email, string taskTitle, IConfiguration config)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(config["Email:FromAddress"]));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "New Task Created";
            message.Body = new TextPart("plain")
            {
                Text = $"A new task titled '{taskTitle}' has been created."
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(config["Email:SmtpServer"], 587, false);
            await smtp.AuthenticateAsync(config["Email:User"], config["Email:Password"]);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }
    }
}
