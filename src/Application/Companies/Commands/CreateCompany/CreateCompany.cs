using System.Security.Claims;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Companies.Commands.CreateCompany;

public record CreateCompanyCommand : IRequest<ResponseDto>
{
    public required string CompanyId { get; init; }
    public required string CompanyName { get; init; }
    public required string Phone { get; init; }
    public required string Email { get; init;}
}

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;

    public CreateCompanyCommandHandler(IApplicationDbContext context, IUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ResponseDto> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if the companyId already exists in the database
            var existingCompany = await _context.Company
                .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId, cancellationToken);

            if (existingCompany != null)
            {
                // Return an error response if the companyId already exists
                return new ResponseDto(409, "Company with the same ID already exists");
            }
            var entity = new Company
            {
                CompanyId = request.CompanyId,
                CompanyName = request.CompanyName,
                PhoneContact = request.Phone,
                EmailContact = request.Email,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = _currentUser.Id,  
                LastModified = DateTimeOffset.UtcNow,
                LastModifiedBy = _currentUser.Id  
            };

            _context.Company.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return new ResponseDto(201, "Company created successfully", entity);
        }
        catch (Exception ex)
        {
            // Handle and log the exception as needed
            return new ResponseDto(500, $"An error occurred while creating the company: {ex.Message}");
        }
    }
}
