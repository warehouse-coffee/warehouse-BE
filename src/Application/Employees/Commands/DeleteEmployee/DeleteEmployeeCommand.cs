using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Employee.Commands.DeleteEmployee;

public class DeleteEmployeeCommand : IRequest<bool>
{
    public required string Id { get; set; }
}
public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;
    public DeleteEmployeeCommandHandler( IIdentityService identityService, IUser currentUser)
    {
        this._identityService = identityService;
        this._currentUser = currentUser;
    }
    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var rs = false;
        var role = await _identityService.GetRoleNamebyUserId(request.Id);
        if(role == "Employee")
        {
            var deleteresult = await _identityService.DeleteUserAsync(request.Id);
            if (deleteresult.Succeeded)
            {
                rs = true;
            }
        }
        
        return rs;

    }


}
