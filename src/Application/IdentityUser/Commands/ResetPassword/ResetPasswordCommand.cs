using Microsoft.AspNetCore.Http;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.IdentityUser.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<ResetPasswordVm>
{
    public required string Email { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
}

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordVm>
    {
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
    public ResetPasswordCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
        {
            this._identityService = identityService;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResetPasswordVm> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
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
            var resetResult = await _identityService.ResetPasswordAsync(request.Email, request.CurrentPassword, request.NewPassword);

            if (!resetResult.Succeeded)
            {
                return new ResetPasswordVm
                {
                    Token = string.Empty,
                    StatusCode = 400 
                };
            }

            var token = await _identityService.SignIn(request.Email, request.NewPassword, sourcePath);
            if (!string.IsNullOrEmpty(token))
            {
                return new ResetPasswordVm
                {
                    Token = token,
                    StatusCode = 200 
                };
            }

            return new ResetPasswordVm
            {
                Token = string.Empty,
                StatusCode = 400 
            };
        }
    }

