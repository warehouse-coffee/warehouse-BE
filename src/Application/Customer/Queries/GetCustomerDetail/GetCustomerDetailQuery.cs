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

}
public class GetCustomerDetailQueryHandler : IRequestHandler<GetCustomerDetailQuery, CustomerDetailVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;

    public GetCustomerDetailQueryHandler(IApplicationDbContext context
        , IMapper mapper
        , IIdentityService identityService)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
    }
    public async Task<CustomerDetailVM> Handle(GetCustomerDetailQuery request, CancellationToken cancellationToken)
    {
        var customer = await _identityService.GetUserNameAsync("");
        return new CustomerDetailVM { };
    }
}
