using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Companies.Queries.CompanyDetail;

public record GetCompanyDetailQuery : IRequest<ResponseDto>
{
    public required string CompanyId { get; init; }
}
public class GetCompanyDetailQueryHandler : IRequestHandler<GetCompanyDetailQuery, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILoggerService _logger;

    public GetCompanyDetailQueryHandler(IApplicationDbContext context, ILoggerService logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ResponseDto> Handle(GetCompanyDetailQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId && !c.IsDeleted, cancellationToken);

            if (company == null)
            {
                _logger.LogWarning($"Company with ID {request.CompanyId} not found or already deleted.");
                return new ResponseDto(404, "Company not found.");
            }

            return new ResponseDto(200, "Company details retrieved successfully.", company);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred in GetCompanyDetailQueryHandler: {ex.Message}", ex);
            return new ResponseDto(500, "An error occurred while retrieving the company details.");
        }
    }
}
