using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Companies.Commands.UpdateCompany;

public record UpdateCompanyCommand : IRequest<ResponseDto>
{
    public required string CompanyId { get; init; }
    public  string? CompanyName { get; init; }
    public  string? Phone { get; init; }
    public  string? Email { get; init; }
}
public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILoggerService _logger;
    private readonly IUser _currentUser;

    public UpdateCompanyCommandHandler(
        IApplicationDbContext context,
        ILoggerService logger,
        IUser currentUser)
    {
        _context = context;
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<ResponseDto> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                _logger.LogWarning("Current user ID is null or empty.");
                return new ResponseDto(400, "User not authenticated.");
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId && !c.IsDeleted, cancellationToken);

            if (company == null)
            {
                _logger.LogWarning($"Company with ID {request.CompanyId} not found or already deleted.");
                return new ResponseDto(404, "Company not found.");
            }

            company.CompanyName = request.CompanyName ?? company.CompanyName;
            company.PhoneContact = request.Phone ?? company.PhoneContact;
            company.EmailContact = request.Email ?? company.EmailContact;

            _context.Companies.Update(company);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation($"Company with ID {request.CompanyId} updated by user {_currentUser.Id}.");

            return new ResponseDto(200, "Company updated successfully.", company);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred in UpdateCompanyCommandHandler: {ex.Message}", ex);
            return new ResponseDto(500, "An error occurred while updating the company.");
        }
    }
}