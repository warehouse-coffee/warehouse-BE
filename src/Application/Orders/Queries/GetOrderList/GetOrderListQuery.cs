using Microsoft.Extensions.Logging;
using System.Reflection;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;
using warehouse_BE.Domain.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace warehouse_BE.Application.Orders.Queries.GetOrderList;

public class GetOrderListQuery : IRequest<OrderListVM>
{
    public Page? Page { get; set; } 
}
public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, OrderListVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetOrderListQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        this._context = context;
        this._mapper = mapper;
    }
    public async Task<OrderListVM> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        var rs = new OrderListVM() { };
        try
        {
            var query = _context.Orders
                            .Where(o => !o.IsDeleted)
                            .AsQueryable();

            //Page
            var totalElements = await query.CountAsync(cancellationToken);

            var orders = await query
               .Skip((request.Page?.PageNumber - 1 ?? 0) * (request.Page?.Size ?? 1)) 
               .Take(request.Page?.Size ?? 10) 
               .Select(o => _mapper.Map<OrderDto>(o)) 
               .ToListAsync(cancellationToken);

            if (orders.Count > 0)
            {
                rs.Orders = orders;
                rs.Page = new Page
                {
                    Size = request.Page?.Size ?? 0,
                    TotalElements = totalElements,
                    PageNumber = request.Page?.PageNumber ?? 1
                };
            }
        } catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while retrieving the order list: {ex.Message}");
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
