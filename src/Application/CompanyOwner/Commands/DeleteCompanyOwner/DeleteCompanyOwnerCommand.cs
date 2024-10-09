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

    private readonly IFileService _fileService;
    public DeleteCompanyOwnerCommandHandler(IApplicationDbContext context, IIdentityService identityService, IFileService fileService)
    {
        this._context = context;
        this._identityService = identityService;
        this._fileService = fileService;
    }
    public async Task<bool> Handle (DeleteCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        var rs = false;
       var user = await _identityService.GetCompanyOwnerByIdAsync(request.UserId);
        if (user != null && user?.ImageFile != null)
        {
            _fileService.DeleteFile(user.ImageFile);
        }
        var data = await _identityService.DeleteUser(request.UserId);
        if (data.Succeeded)
        {
            rs = true;
        }
        return rs;
    }
}
