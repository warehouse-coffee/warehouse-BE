using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Inventories.Queries.GetInventoriesByStorage;

public class GetInventoriesByStorageQuery : IRequest<InventoryListVM>
{
    public required int StorageId { get; set; }
    public required Page Page { get; set; }
    public List<FilterData>? Filters { get; set; }
}
public class GetInventoriesByStorageQueryHandler : IRequestHandler<GetInventoriesByStorageQuery, InventoryListVM>
{
    private readonly IApplicationDbContext _context;
    private readonly ILoggerService _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    private readonly IFilterData _filterData;
    private readonly IMapper _mapper;
    public GetInventoriesByStorageQueryHandler(IApplicationDbContext context, ILoggerService logger, IUser user, IIdentityService identityService, IFilterData filterData, IMapper mapper)
    {
        _context = context;
        _logger = logger;
        _user = user;
        _identityService = identityService;
        _filterData = filterData;
        _mapper = mapper;
    }

    public async Task<InventoryListVM> Handle(GetInventoriesByStorageQuery request, CancellationToken cancellationToken)
    {
        var result = new InventoryListVM();
       

        try
        {
            if (_user.Id != null)
            {
                var userStorages = await _identityService.GetUserStoragesAsync(_user.Id);
                // check storage Id must be in list storages of the user 
                if (!userStorages.Any(s => s.Id == request.StorageId))
                {
                    _logger.LogWarning($"User {_user.Id} attempted to access unauthorized storage {request.StorageId}.");
                    return result;
                }

                var query = _context.Inventories
                .Where(i => i.StorageId == request.StorageId)
                .AsQueryable();

                
                    // Apply filters dynamically
                    if (request.Filters != null && request.Filters.Any())
                    {
                        query = _filterData.HandleFilterData(query, request.Filters);
                    }

                    // Apply sorting if specified
                    if (!string.IsNullOrEmpty(request.Page.SortBy))
                    {
                        query = _filterData.HandleSort(query, request.Page.SortBy, request.Page.SortAsc);
                    }

                    if (query != null)
                    {
                        request.Page.TotalElements = await query.CountAsync(cancellationToken);
                    }
                if (query != null && query.Any())  
                {
                    // Apply pagination
                    var inventories = await query
                        .Skip((request.Page.PageNumber - 1) * request.Page.Size)
                        .Take(request.Page.Size)
                        .ToListAsync(cancellationToken);

                  result.Inventories = inventories.Select(i => _mapper.Map<InventoryDto>(i)).ToList();

                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error in GetInventoriesByStorageQueryHandler: ", ex);
        }
        result.Page = request.Page;

        return result;

    }
}
