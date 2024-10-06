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
    public string? SearchText { get; set; } 
    public List<FilterData>? FilterData { get; set; } 
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
            var query = _context.Orders.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(request.SearchText))
            {
                query = query.Where(o => o.Type.Contains(request.SearchText));
            }

            // Filter
            if (request.FilterData != null)
            {

                foreach (var filter in request.FilterData)
                {
                    if (!string.IsNullOrEmpty(filter.Prop) && filter.Value != null)
                    {
                        var propertyInfo = typeof(Order).GetProperty(filter.Prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        if (propertyInfo == null)
                        {
                            return rs;
                        }

                        if (!string.IsNullOrEmpty(filter.Prop) && filter.Value != null)
                        {
                            switch (filter.Filter)
                            {
                                case "Equals":
                                    if (propertyInfo.PropertyType == typeof(string))
                                    {
                                        query = query.Where(o => EF.Property<string>(o, filter.Prop) == filter.Value.ToString());
                                    }
                                    break;
                                case "Contains":
                                    if (propertyInfo.PropertyType == typeof(string))
                                    {
                                        query = query.Where(o => EF.Property<string>(o, filter.Prop).Contains(filter.Value.ToString()));
                                    }
                                    break;
                                case "GreaterThan":
                                    if (propertyInfo.PropertyType == typeof(decimal) && decimal.TryParse(filter.Value.ToString(), out var decimalValue))
                                    {
                                        query = query.Where(o => EF.Property<decimal>(o, filter.Prop) > decimalValue);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            // Sort
            if (request.Page != null && !string.IsNullOrWhiteSpace(request.Page.SortBy))
            {
                var sortPropertyInfo = typeof(Order).GetProperty(request.Page.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (sortPropertyInfo != null) 
                {
                    query = request.Page.SortAsc ?
                        query.OrderBy(o => EF.Property<object>(o, request.Page.SortBy)) :
                        query.OrderByDescending(o => EF.Property<object>(o, request.Page.SortBy));
                }
            }

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
