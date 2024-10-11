
using System.Net;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.IdentityUser.Commands.CreateUser;

public record CreateUserCommand : IRequest<ResponseDto>
{
    public required UserRegister UserRegister { get; init; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    public CreateUserCommandHandler(IApplicationDbContext context, IIdentityService identityService, IEmailService emailService)
    {
        _context = context;
        _identityService = identityService;
        _emailService = emailService;
    }

    public async Task<ResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var userRegister = request.UserRegister;

        if (string.IsNullOrWhiteSpace(userRegister.UserName) ||
            string.IsNullOrWhiteSpace(userRegister.Password) ||
            string.IsNullOrWhiteSpace(userRegister.Email) ||
            string.IsNullOrWhiteSpace(userRegister.PhoneNumber) ||
            string.IsNullOrWhiteSpace(userRegister.CompanyId) ||
            string.IsNullOrWhiteSpace(userRegister.RoleName))
        {
            return new ResponseDto(400, "All fields are required.");
        }
        var existingCompany = await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyId == userRegister.CompanyId, cancellationToken);

        if (existingCompany == null)
        {
            var entity = new Company
            {
                CompanyId = userRegister.CompanyId,
            };

            _context.Companies.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);
        }

        var registerResult = await _identityService.RegisterAsync(userRegister);
        var result = registerResult.Result;
        var userId = registerResult.UserId;

        if (result.Succeeded)
        {
            try
            {


                var resetLink = $"https://yourapp.com/reset-password?email={userRegister.Email}";

                // Nội dung email
                var subject = "Welcome to Our Service!";
                var body = $"<p>Dear {userRegister.UserName},</p>" +
                           $"<p>Your account has been created successfully. Here are your details:</p>" +
                           $"<p>Username: {userRegister.UserName}</p>" +
                           $"<p>Password: {userRegister.Password}</p>" +
                           $"<p>Please click <a href='{resetLink}'>here</a> to reset your password.</p>";

                // Gửi email
                await _emailService.SendEmailAsync(userRegister.Email, subject, body);
                return new ResponseDto(200, "User registered successfully.", new { UserId = userId });
            }
            catch
            {
                return new ResponseDto(400, "Send mail Fail.");
            }
        }

        // Kiểm tra kiểu dữ liệu của result.Errors
        if (result.Errors != null && result.Errors.Any())
        {
            var errors = result.Errors.ToList();
            var errorMessage = string.Join(", ", errors);
            return new ResponseDto(400, $"User registration failed: {errorMessage}");
        }

        return new ResponseDto(400, "User registration failed due to unknown error.");
    }


}
