using MediatR;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Customer.Queries.GetListCustomer;

namespace warehouse_BE.Application.Customer.Queries.GetLlistCustomer;

public class GetListCustomerQuery : IRequest<CustomerListVM>
{

}
public class GetListCustomerQueryHandler : IRequestHandler<GetListCustomerQuery, CustomerListVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityService _identityService;

    public GetListCustomerQueryHandler(IApplicationDbContext context
        , IMapper mapper
        ,IIdentityService identityService)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
    }
    public async Task<CustomerListVM> Handle(GetListCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await _identityService.GetUserNameAsync("");
        return new CustomerListVM { };
    }
}
