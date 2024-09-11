using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using warehouse_BE.Application.Common.Interfaces;
using System.Net.Http;

namespace warehouse_BE.Infrastructure.Service;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (!int.TryParse(_configuration["EmailSettings:SmtpPort"], out var smtpPort))
        {
            throw new ArgumentException("Invalid or missing SmtpPort in configuration.");
        }

        var smtpHost = _configuration["EmailSettings:SmtpHost"];
        var smtpUser = _configuration["EmailSettings:SmtpUser"] ?? throw new ArgumentNullException("SmtpUser is null");
        var smtpPass = _configuration["EmailSettings:SmtpPass"];

        using (var client = new SmtpClient(smtpHost, smtpPort))
        {
            client.Credentials = new NetworkCredential(smtpUser, smtpPass);
            client.EnableSsl = true;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(to);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}