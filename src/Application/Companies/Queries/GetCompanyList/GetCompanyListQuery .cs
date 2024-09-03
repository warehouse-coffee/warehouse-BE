using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Companies.Queries.GetCompanyList;

public class GetCompanyListQuery : IRequest<ResponseDto>
{
}

public class GetCompanyListQueryHandler : IRequestHandler<GetCompanyListQuery, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCompanyListQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResponseDto> Handle(GetCompanyListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var companyDtos = await _context.Company
                .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            if (companyDtos == null || !companyDtos.Any())
            {
                return new ResponseDto(404, "No companies found");
            }

            // Create a ResponseDto with the company list
            var responseDto = new ResponseDto(200, "Companies retrieved successfully", new CompanyListVM
            {
                CompanyList = companyDtos
            });

            return responseDto;
        }
        catch (AutoMapperMappingException ex)
        {
            // Log the exception (you can use a logging framework here)
             Console.WriteLine($"AutoMapperMappingException: {ex.Message}");
            return new ResponseDto(500, "An error occurred while mapping company data.");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Exception: {ex.Message}");
            return new ResponseDto(500, "An unexpected error occurred.");
        }
    }
}
