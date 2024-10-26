using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Customer.Commands.DeleteCustomer;

public class DeleteCustomerCommand : IRequest<bool>
{
    public required string UserId { get; set; }
}
public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;
    public DeleteCustomerCommandHandler( IIdentityService identityService, IUser currentUser)
    {
        this._identityService = identityService;
        this._currentUser = currentUser;
    }
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var rs = false;
        var role = await _identityService.GetRoleNamebyUserId(request.UserId);
        if(role == "Customer")
        {
            var deleteresult = await _identityService.DeleteUserAsync(request.UserId);
            if (deleteresult.Succeeded)
            {
                rs = true;
            }
        }
        
        return rs;

    }


}
