using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.IdentityUser.Commands.SignIn;

public record SignInCommand : IRequest<SignInVm>
{
    public string? Email { get; init; }

    public string? Password { get; init; }
}

public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInVm>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IAntiforgery _forgeryService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SignInCommandHandler(IApplicationDbContext context, IIdentityService identityService, IAntiforgery antiforgery, IHttpContextAccessor httpContextAccessor)
    {
        this._context = context;
        this._identityService = identityService;
        this._forgeryService = antiforgery;
        this._httpContextAccessor = httpContextAccessor;
    }

    public async Task<SignInVm> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request?.Email) && !string.IsNullOrEmpty(request?.Password))
        {
            var sourcePath = string.Empty;
            if (_httpContextAccessor.HttpContext != null &&
                 _httpContextAccessor.HttpContext.Request != null)
            {
                var requestContext = _httpContextAccessor.HttpContext.Request;
                var scheme = requestContext.Scheme;
                var host = requestContext.Host.Value;
                sourcePath = $"{scheme}://{host}/Resources/";
            }
            var token = await _identityService.SignIn(request.Email, request.Password, sourcePath);
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
