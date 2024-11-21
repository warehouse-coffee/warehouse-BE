using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;
using warehouse_BE.Application.Inventories.Queries.GetInventoriesByStorage;
using warehouse_BE.Application.Orders.Queries.GetOrderList;
namespace warehouse_BE.Application.Orders.Queries.GetSaleOrderList;

public class GetSaleOrderListQuery : IRequest<OrderListVM>
{
    public required Page Page { get; set; }
    public List<FilterData>? Filters { get; set; }
}
public class GetSaleOrderListQueryHandler : IRequestHandler<GetSaleOrderListQuery, OrderListVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILoggerService _loggerService;
    private readonly IFilterData _filterData;
    public GetSaleOrderListQueryHandler(IApplicationDbContext context, IMapper mapper, ILoggerService loggerService, IFilterData filterData)
    {
        this._context = context;
        this._mapper = mapper;
        this._loggerService = loggerService;
        this._filterData = filterData;
    }
    public async Task<OrderListVM> Handle(GetSaleOrderListQuery request, CancellationToken cancellationToken)
    {
        var rs = new OrderListVM() { };
        try
        {
            var query = _context.Orders
                            .Where(o => !o.IsDeleted & o.Type == "Sale")
                            .AsQueryable();

            // Apply filters dynamically
            if (request.Filters != null && request.Filters.Any())
            {
                query = _filterData.HandleFilterData(query, request.Filters);
            }

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(request.Page?.SortBy))
            {
                query = _filterData.HandleSort(query, request.Page.SortBy, request.Page.SortAsc);
            }

            if (query != null && request.Page != null)
            {
                request.Page.TotalElements = await query.CountAsync(cancellationToken);
            }
            if (query != null && query.Any()  && request.Page != null)
            {
                // Apply pagination
                var order = await query
                    .Skip((request.Page.PageNumber - 1) * request.Page.Size)
                    .Take(request.Page.Size)
                    .ToListAsync(cancellationToken);

                rs.Orders = order.Select(i => _mapper.Map<OrderDto>(i)).ToList();
                rs.Page = request.Page;
            }
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Error at GetSaleOrderListQuery : ", ex);
            rs.Orders = new List<OrderDto>();
            rs.Page = new Page
            {
                Size = 0,
                TotalElements = 0,
                PageNumber = 1
            };
        }
        return rs;

    }
}