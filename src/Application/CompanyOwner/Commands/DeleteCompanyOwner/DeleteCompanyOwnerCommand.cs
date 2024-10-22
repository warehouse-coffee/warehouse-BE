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
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly IFileService _fileService;
    public DeleteCompanyOwnerCommandHandler(IUser currentUser, IIdentityService identityService, IFileService fileService)
    {
        this._currentUser = currentUser;
        this._identityService = identityService;
        this._fileService = fileService;
    }
    public async Task<bool> Handle (DeleteCompanyOwnerCommand request, CancellationToken cancellationToken)
    {
        var rs = false;
        if(_currentUser.Id == null)
        {
            return rs;
        }
        var role = await _identityService.GetRoleNamebyUserId(request.UserId);
        var currentRole = await _identityService.GetRoleNamebyUserId(_currentUser.Id);
        if (role != null && role != currentRole)
        {
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
        }
      
        return rs;
    }
}
