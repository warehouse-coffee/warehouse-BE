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
    private readonly IUser _currentUser;
    public GetListCustomerQueryHandler(IApplicationDbContext context
        , IMapper mapper
        ,IIdentityService identityService
        ,IUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _identityService = identityService;
        _currentUser = currentUser;
    }
    public async Task<CustomerListVM> Handle(GetListCustomerQuery request, CancellationToken cancellationToken)
    {
        var rs = new CustomerListVM();
        if (string.IsNullOrEmpty(_currentUser?.Id))
        {
            return rs;
        }
        var companyIdResult = await _identityService.GetCompanyId(_currentUser.Id);
        if(companyIdResult.CompanyId != null)
        {
            var customer = await _identityService.GetUsersByRoleAsync("Customer",companyIdResult.CompanyId);
            if (customer.Count > 0)
            {
                rs.Customers = customer;
            }
        }
        return rs;
    }
}
