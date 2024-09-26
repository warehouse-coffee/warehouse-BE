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
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public DeleteCustomerCommandHandler(IApplicationDbContext context
        , IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var rs = false;
        var deleteresult = await _identityService.DeleteUserAsync(request.UserId);
        if(deleteresult.Succeeded)
        {
            rs = true;
        }
        return rs;

    }


}
