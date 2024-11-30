using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<ResponseDto>
{
    public int CustomerId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}
public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILoggerService _logger;
    private readonly IUser _currentUser;

    public UpdateCustomerCommandHandler(
        IApplicationDbContext context,
        ILoggerService logger,
        IUser currentUser)
    {
        _context = context;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<ResponseDto> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                _logger.LogWarning("Current user ID is null or empty.");
                return new ResponseDto(400, "User not authenticated.");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId && !c.IsDeleted, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning($"Customer with ID {request.CustomerId} not found or already deleted.");
                return new ResponseDto(404, "Customer not found.");
            }

            // Update customer properties
            customer.Name = request.Name ?? customer.Name;
            customer.Email = request.Email ?? customer.Email;
            customer.Phone = request.Phone ?? customer.Phone;
            customer.Address = request.Address ?? customer.Address;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Customer with ID {request.CustomerId} updated by user {_currentUser.Id}.");

            return new ResponseDto(200, "Customer updated successfully.", customer);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred in UpdateCustomerCommandHandler: {ex.Message}", ex);
            return new ResponseDto(500, "An error occurred while updating the customer.");
        }
    }
}