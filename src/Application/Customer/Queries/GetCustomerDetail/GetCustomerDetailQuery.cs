using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Customer.Queries.GetListCustomer;
using warehouse_BE.Application.Customer.Queries.GetLlistCustomer;

namespace warehouse_BE.Application.Customer.Queries.GetCustomerDetail;

public class GetCustomerDetailQuery : IRequest<CustomerDetailVM>
{
    public required string UserId { get; set; }
}
public class GetCustomerDetailQueryHandler : IRequestHandler<GetCustomerDetailQuery, CustomerDetailVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetCustomerDetailQueryHandler(IApplicationDbContext context
        , IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }
    public async Task<CustomerDetailVM> Handle(GetCustomerDetailQuery request, CancellationToken cancellationToken)
    {
        var rs = new CustomerDetailVM { };
        rs = await _identityService.GetUserByIdAsync(request.UserId);
        if (rs == null)
        {
           return new CustomerDetailVM { };
        }
        var company = await _context.Company
            .FirstOrDefaultAsync(c => c.CompanyId == rs.CompanyId, cancellationToken);

        if (company != null)
        {
            rs.CompanyName = company.CompanyName;
            rs.CompanyPhone = company.PhoneContact;
            rs.CompanyEmail = company.EmailContact;
            rs.CompanyAddress = company.Address;
        }
        return rs;
    }
}
