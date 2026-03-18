using MailKit.Net.Smtp;
using MimeKit;

namespace FirstWebApp.Services
{
    public class EmailService
    {
        public void SendOtp(string toEmail, string otp)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("FirstWebApp", "shubhamsinha9257@gmail.com"));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Login OTP";

            message.Body = new TextPart("plain")
            {
                Text = "Your OTP is: " + otp
            };

            using var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

            client.Authenticate("shubhamsinha9257@gmail.com", "piofafwkbnvrxdgj");

            client.Send(message);
            client.Disconnect(true);
        }
    }
}