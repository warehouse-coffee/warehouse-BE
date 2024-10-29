using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Customer.Queries.GetListCustomer;
using warehouse_BE.Application.Customer.Queries.GetLlistCustomer;
using warehouse_BE.Application.Storages.Queries.GetStorageOfUser;

namespace warehouse_BE.Application.Customer.Queries.GetCustomerDetail;

public class GetCustomerDetailQuery : IRequest<CustomerDetailVM>
{
    public required string Id { get; set; }
}
public class GetCustomerDetailQueryHandler : IRequestHandler<GetCustomerDetailQuery, CustomerDetailVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;
    private readonly IUser _currentUser;
    private readonly ILoggerService _logger;
    public GetCustomerDetailQueryHandler(IApplicationDbContext context
        , IIdentityService identityService
        , IMapper mapper
        , IUser currentUser
        , ILoggerService logger)
    {
        _context = context;
        _identityService = identityService;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }
    public async Task<CustomerDetailVM> Handle(GetCustomerDetailQuery request, CancellationToken cancellationToken)
    {
        var rs = new CustomerDetailVM { };
        if (string.IsNullOrEmpty(_currentUser?.Id))
        {
            return rs;
        }
        rs = await _identityService.GetUserByIdAsync(request.Id);
        if (rs == null)
        {
           return new CustomerDetailVM { };
        }
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.CompanyId == rs.CompanyId, cancellationToken);

        if (company != null)
        {
            rs.CompanyName = company.CompanyName;
            rs.CompanyPhone = company.PhoneContact;
            rs.CompanyEmail = company.EmailContact;
            rs.CompanyAddress = company.Address;
        }
        
        var storages = await _identityService.GetUserStoragesAsync(request.Id);

        var storageDtos = _mapper.Map<List<StorageDto>>(storages);
        rs.Storages = storageDtos;
        return rs;
    }
}
