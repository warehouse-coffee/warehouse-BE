using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Customers.Queries.CustomerDetail;

public class GetCustomerDetailQuery : IRequest<CustomerDetailDto>
{
    public int CustomerId { get; set; }
}
public class GetCustomerDetailQueryHandler : IRequestHandler<GetCustomerDetailQuery, CustomerDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILoggerService _logger;

    public GetCustomerDetailQueryHandler(IApplicationDbContext context, IMapper mapper, ILoggerService logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CustomerDetailDto> Handle(GetCustomerDetailQuery request, CancellationToken cancellationToken)
    {
        try
        {

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId && !c.IsDeleted, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning($"Customer not found with ID: {request.CustomerId}");
                throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");
            }

            _logger.LogInformation($"Successfully fetched details for customer ID: {request.CustomerId}");

            return _mapper.Map<CustomerDetailDto>(customer);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"KeyNotFoundException: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error while fetching customer ID {request.CustomerId}: ",ex);
            throw new ApplicationException("An error occurred while fetching customer details.", ex);
        }
    }
}