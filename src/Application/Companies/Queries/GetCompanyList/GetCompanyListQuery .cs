using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;

namespace warehouse_BE.Application.Companies.Queries.GetCompanyList;

public class GetCompanyListQuery : IRequest<CompanyListVM>
{
}

public class GetCompanyListQueryHandler : IRequestHandler<GetCompanyListQuery, CompanyListVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCompanyListQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<CompanyListVM> Handle(GetCompanyListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var companyDtos = await _context.Companies
                .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            if (companyDtos == null || !companyDtos.Any())
            {
                return new CompanyListVM();
            }

            // Create a ResponseDto with the company list
            var responseDto =  new CompanyListVM()
            {
                CompanyList = companyDtos
            };

            return responseDto;
        }
        catch (AutoMapperMappingException ex)
        {
            // Log the exception (you can use a logging framework here)
             Console.WriteLine($"AutoMapperMappingException: {ex.Message}");
            return new CompanyListVM();
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Exception: {ex.Message}");
            return new CompanyListVM();
        }
    }
}
