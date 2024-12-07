using System;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.IdentityUser.Commands.CreateUser;

namespace warehouse_BE.Application.IdentityUser.Queries.CheckEmail;

public class CheckEmailQuery : IRequest<bool>
{
    public string? Email { get; set; }
}
public class CheckEmailQueryHandler : IRequestHandler<CheckEmailQuery, bool>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly Random _random;
    public CheckEmailQueryHandler(IIdentityService identityService,IEmailService emailService, Random random)
    {
        _identityService = identityService;
        _emailService = emailService;
        _random = random;
    }
    public async Task<bool> Handle(CheckEmailQuery request, CancellationToken cancellationToken)
    {
        int rs = 0;
        if(request.Email != null) 
        {
            var user = await _identityService.GetUserIdByEmail(request.Email);
            if(!string.IsNullOrEmpty(user))
            {
                var newPassword = GenerateRandomPassword();

                // Set the new password for the user using the user ID
                var setPasswordResult = await _identityService.SetNewPasswordAsync(user, newPassword);
                if (!setPasswordResult)
                {
                    return false;
                }

                var resetLink = $"https://warehouse-frontend-delta.vercel.app/reset-password";

                // Nội dung email
                var subject = "Welcome to Our Service!";
                var body = $"<p>Dear User,</p>" +
                           $"<p>Your account has been created successfully. Here are your details:</p>" +
                $"<p>Username: {request.Email}</p>" +
                           $"<p>Password: <strong>{newPassword}</strong></p>" +
                           $"<p>Please click <a href='{resetLink}'>here</a> to reset your password.</p>";

                // Gửi email
                await _emailService.SendEmailAsync(request.Email, subject, body);

                rs = 1;

            }
        }
        return rs > 0;
    }
    private string GenerateRandomPassword()
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
        var length = 12; // Length of the password
        var password = new char[length];
        for (int i = 0; i < length; i++)
        {
            password[i] = validChars[_random.Next(validChars.Length)];
        }
        return new string(password);
    }
}