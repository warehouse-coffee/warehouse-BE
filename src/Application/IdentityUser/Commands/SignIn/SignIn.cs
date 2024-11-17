using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.IdentityUser.Commands.SignIn;

public record SignInCommand : IRequest<SignInVm>
{
    public string? Email { get; init; }

    public string? Password { get; init; }
}

public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInVm>
{
    private readonly ILoggerService _logger;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public SignInCommandHandler( IIdentityService identityService, IHttpContextAccessor httpContextAccessor, ILoggerService loggerService)
    {
        this._identityService = identityService;
        this._httpContextAccessor = httpContextAccessor;
        this._logger = loggerService;
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
                _logger.LogInformation("SignIn - User SignIn with : "+ request.Email );
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
