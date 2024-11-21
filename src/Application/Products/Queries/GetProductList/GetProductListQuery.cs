using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;
namespace warehouse_BE.Application.Products.Queries.GetProductList;

public class GetProductListQuery : IRequest<ProductListVM>
{
    public required Page Page { get; set; }
    public List<FilterData>? Filters { get; set; }
}
public class GetPrpductListQueryhandler : IRequestHandler<GetProductListQuery, ProductListVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFilterData _filterData;
    private readonly IIdentityService _identityService;
    private readonly ILoggerService _loggerService;
    private readonly IUser _user;

    public GetPrpductListQueryhandler (IApplicationDbContext context, IMapper mapper, IFilterData filterData, IIdentityService identityService, ILoggerService loggerService, IUser user)
    {
        _context = context;
        _mapper = mapper;
        _filterData = filterData;
        _identityService = identityService;
        _loggerService = loggerService;
        _user = user;
    }

    public async Task<ProductListVM> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        var rs = new ProductListVM();
        
        try
        {
            if (_user.Id != null)
            {
                var userStorages = await _identityService.GetUserStoragesAsync(_user.Id);
                var storageIds = userStorages.Select(s => s.Id).ToList();

                var query = _context.Products
                   .Where(p => storageIds.Contains(p.StorageId))
                   .AsQueryable();

                if (request.Filters != null && request.Filters.Any())
                {
                    query = _filterData.HandleFilterData(query, request.Filters);
                }

                if (!string.IsNullOrEmpty(request.Page.SortBy))
                {
                    query = _filterData.HandleSort(query, request.Page.SortBy, request.Page.SortAsc);
                }

                request.Page.TotalElements = await query.CountAsync(cancellationToken);

                var products = await query
                    .Skip((request.Page.PageNumber - 1) * request.Page.Size)
                    .Take(request.Page.Size)
                    .ToListAsync(cancellationToken);

                rs.ProductList = products.Select(p => _mapper.Map<ProductDto>(p)).ToList();
                rs.Page = request.Page;
            }
        }
        catch(Exception ex)
        {
            _loggerService.LogError("Error at GetProductListQuery: ", ex);
        }
        
        return rs;
    }
}
