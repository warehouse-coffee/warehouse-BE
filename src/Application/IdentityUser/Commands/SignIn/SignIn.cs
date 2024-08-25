using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.IdentityUser.Commands.SignIn;

public record SignInCommand : IRequest<SignInVm>
{
    public string? UserName { get; init; }

    public string? Password { get; init; }
}

public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    public SignInCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<SignInVm> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request?.UserName) && !string.IsNullOrEmpty(request?.Password))
        {
            var token = await _identityService.SignIn(request.UserName, request.Password);
            if (!string.IsNullOrEmpty(token))
            {
                return new SignInVm
                {
                    Token = token,
                    StatusCode = 200
                };
            }
        }

        
        return new SignInVm
        {
            Token = string.Empty,
            StatusCode = 400
        };

    }
}
