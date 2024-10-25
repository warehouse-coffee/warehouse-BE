using warehouse_BE.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;
namespace warehouse_BE.Application.IdentityUser.Commands.LogOut;

public class LogoutCommand : IRequest<bool>
{
    public required string UserId { get; set; }
}
public class LogoutCommandHandler : IRequestHandler<LogoutCommand,bool>
{
    private readonly IIdentityService _identityService;

    public LogoutCommandHandler(IIdentityService identityService)
    {
        this._identityService = identityService;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        int rs = 0;

        var result = await _identityService.Logout(request.UserId);
        if(result)
        {
            rs++;
        }
        return rs > 0;
    }
}