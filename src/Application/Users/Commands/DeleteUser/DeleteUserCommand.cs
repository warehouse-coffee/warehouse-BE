using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<bool>
{
    public required string UserId;
}
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly IFileService _fileService;
    public DeleteUserCommandHandler(IIdentityService identityService, IFileService fileService)
    {
        _identityService = identityService;
        _fileService = fileService;
    }
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
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