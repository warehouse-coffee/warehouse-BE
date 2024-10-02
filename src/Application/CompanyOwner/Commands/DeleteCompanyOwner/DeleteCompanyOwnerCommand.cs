using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.CompanyOwner.Commands.DeleteCompanyOwner;

public class DeleteCompanyOwnerCommand : IRequest<bool>
{
    public required string UserId { get; set; }
}
public class DeleteCompanyOwnerCommandHandler : IRequestHandler<DeleteCompanyOwnerCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    public DeleteCompanyOwnerCommandHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        this._context = context;
        this._identityService = identityService;
    }
    public async Task<bool> Handle (DeleteCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        var rs = false;
        var data = await _identityService.DeleteUser(request.UserId);
        if (data.Succeeded)
        {
            rs = true;
        }
        return rs;
    }
}
