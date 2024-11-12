using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Inventories.Queries.GetListProductOfInventory;

public class GetListProductOfInventory : IRequest<InventoryProductsListVM>
{
    public Page? Page { get; set; }
    
}
public class GetListProductOfInventoryHandler : IRequestHandler<GetListProductOfInventory, InventoryProductsListVM> 
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    public GetListProductOfInventoryHandler(IApplicationDbContext context, IMapper mapper,IUser user, IIdentityService identityService)
    {
        _context = context; 
        _mapper = mapper;
        _currentUser = user;
        _identityService = identityService;
    }
    public async Task<InventoryProductsListVM> Handle (GetListProductOfInventory request, CancellationToken cancellationToken)
    {
        var rs = new InventoryProductsListVM();
        if (_currentUser.Id != null)
        {
            var userStorages = await _identityService.GetUserStoragesAsync(_currentUser.Id);
            var userStorageIds = userStorages.Select(s => s.Id).ToList();

            var query =  _context.Inventories
                .Where(i => userStorageIds.Contains(i.StorageId) && !i.IsDeleted)
                .Include(i => i.Storage)
                .AsNoTracking();

            if (request.Page != null)
            {
                int pageSize = request.Page.Size;
                int pageNumber = request.Page.PageNumber;

                var totalItems = await query.CountAsync(cancellationToken);
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                query = query
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize);

                var items = await query.ToListAsync(cancellationToken);

                return new InventoryProductsListVM
                {
                    Products = _mapper.Map<List<InventoryProductsDto>>(items),
                    Page = new Page
                    {
                        PageNumber = pageNumber,
                        Size = pageSize,
                        TotalElements = totalItems
                    }
                };
            }

            var allItems = await query.ToListAsync(cancellationToken);

            rs.Products = _mapper.Map<List<InventoryProductsDto>>(allItems);
        }
        return rs;
    }
}
