using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Customers.Queries.GetListCustomerOfUser;

public class GetListCustomerOfUserQuery : IRequest<CustomersVM>
{
}
public class GetListCustomerOfUserQueryHandler : IRequestHandler<GetListCustomerOfUserQuery, CustomersVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;

    public GetListCustomerOfUserQueryHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ILoggerService logger,
        IUser currentUser, 
        IIdentityService identityService)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<CustomersVM> Handle(GetListCustomerOfUserQuery request, CancellationToken cancellationToken)
    {
        var result = new CustomersVM();
        try
        {
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                _logger.LogWarning("Current user ID is null or empty.");
                return result;
            }

            var companyId = await _identityService.GetCompanyIDInt(_currentUser.Id);

            if (companyId == 0)
            {
                _logger.LogWarning($"Company ID not found for user ID: {_currentUser.Id}");
                return result;
            }

            // Fetch all customers with the same company ID
            var customers = await _context.Customers
                .Where(c => c.CompanyId == companyId && !c.IsDeleted)
                .ToListAsync(cancellationToken);

            // Map customers to the view model
            result.Customers = customers.Select(c => _mapper.Map<CustomerDto>(c)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred in GetListCustomerOfUserQueryHandler:", ex);
        }

        return result;
    }
   
}