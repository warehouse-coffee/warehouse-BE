
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.IdentityUser.Commands.CreateUser;

public record CreateUserCommand : IRequest<ResponseDto>
{
    public required UserRegister UserRegister { get; init; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public CreateUserCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
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

        // Gọi phương thức RegisterAsync và nhận kết quả
        var registerResult = await _identityService.RegisterAsync(userRegister);
        var result = registerResult.Result;
        var userId = registerResult.UserId;

        if (result.Succeeded)
        {
            return new ResponseDto(200, "User registered successfully.", new { UserId = userId });
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
