using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WorkingMVC.Services;

public class EmailSenderService : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string code)
    {
        var fromAddress = new MailAddress("quizzytests@gmail.com", "Quizzy");
        var toAddress = new MailAddress(email, "Dear user");
        const string fromPassword = "eplc imyv edac hxzn";
        string body = $"Hello dear user, here is your code - {code}, copy it and paste it on our website";

        var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        };

        try
        {
            smtp.Send(message);
            Console.WriteLine("Email sent successfully.");
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return Task.CompletedTask;
        }
    }
}