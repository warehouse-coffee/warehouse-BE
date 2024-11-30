using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommand : IRequest<ResponseDto>
{
    public int Id { get; set; }
}
public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILoggerService _logger;
    private readonly IUser _currentUser;

    public DeleteCustomerCommandHandler(
        IApplicationDbContext context,
        ILoggerService logger,
        IUser currentUser)
    {
        _context = context;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<ResponseDto> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                _logger.LogWarning("Current user ID is null or empty.");
                return new ResponseDto(400, "User not authenticated.");
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning($"Customer with ID {request.Id} not found or already deleted.");
                return new ResponseDto(404, "Customer not found.");
            }

            customer.IsDeleted = true;

            _context.Customers.Update(customer);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Customer with ID {request.Id} deleted by user {_currentUser.Id}.");

            return new ResponseDto(200, "Customer deleted successfully.", customer);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred in DeleteCustomerCommandHandler: {ex.Message}", ex);
            return new ResponseDto(500, "An error occurred while deleting the customer.");
        }
    }
}