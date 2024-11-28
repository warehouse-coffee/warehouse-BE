using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Products.Queries.GetProductOrder;

public class GetProductsOrder : IRequest<ProductsOrderVM>
{

}
public class GetProductsOrderHandler : IRequestHandler<GetProductsOrder, ProductsOrderVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ILoggerService _loggerService;
    private readonly IMapper _mapper;
    private readonly IUser _user;
    public GetProductsOrderHandler(IApplicationDbContext applicationDbContext, IIdentityService identityService, ILoggerService loggerService, IMapper mapper, IUser user)
    {
        _user = user;
        _context = applicationDbContext;
        _identityService = identityService;
        _loggerService = loggerService;
        _mapper = mapper;
    } 
    public async Task<ProductsOrderVM> Handle(GetProductsOrder request, CancellationToken cancellationToken)
    {
        var rs = new ProductsOrderVM();
        if(_user.Id != null)
        {
            try
            {
                var storage = await _identityService.GetUserStoragesAsync(_user.Id);
                if (storage != null)
                {
                    var storageIDs = storage.Select(x => x.Id).ToList();
                    var query = _context.Inventories.AsQueryable().Where(p => storageIDs.Contains(p.StorageId) && !p.IsDeleted).GroupBy(p => p.ProductName);

                    var products = await _mapper.ProjectTo<ProductDto>(query).ToListAsync();
                    if(products != null)
                    {
                        rs.Products = products;
                    }
                }
            }catch (Exception ex) 
            {
                _loggerService.LogError("Error at GetProductsOrder : ", ex);
            }

        }

        return rs;
    }
}