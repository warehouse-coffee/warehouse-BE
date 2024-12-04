using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.IdentityUser.Queries.CheckEmail;

public class CheckEmailQuery : IRequest<bool>
{
    public string? Email { get; set; }
}
public class CheckEmailQueryHandler : IRequestHandler<CheckEmailQuery, bool>
{
    private readonly IIdentityService _identityService; 
    public CheckEmailQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public async Task<bool> Handle(CheckEmailQuery request, CancellationToken cancellationToken)
    {
        int rs = 0;
        if(request.Email != null) 
        {
            var user = await _identityService.GetUserIdByEmail(request.Email);
            if(!string.IsNullOrEmpty(user))
            {
                rs = 1;
            }
        }
        return rs > 0;
    }
}