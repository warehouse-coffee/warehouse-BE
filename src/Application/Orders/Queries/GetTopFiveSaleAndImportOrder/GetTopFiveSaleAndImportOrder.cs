
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Orders.Queries.GetTopFiveSaleAndImportOrder;

public class GetTopFiveSaleAndImportOrder : IRequest<SaleAndImportOrderVM>
{
}
public class GetTopFiveSaleAndImportOrderHandler : IRequestHandler<GetTopFiveSaleAndImportOrder, SaleAndImportOrderVM>
{
    private readonly IUser _user;
    private  readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ILoggerService _loggerService;

    public GetTopFiveSaleAndImportOrderHandler(IUser user, IApplicationDbContext context, IIdentityService identityService, ILoggerService loggerService)
    {
        _user = user;
        _context = context;
        _identityService = identityService;
        _loggerService = loggerService;
    }
    public async Task<SaleAndImportOrderVM> Handle(GetTopFiveSaleAndImportOrder request, CancellationToken cancellationToken)
    {
        var rs = new SaleAndImportOrderVM();
        if(_user.Id != null)
        {
            try
            {
                var (result, companyId) = await _identityService.GetCompanyId(_user.Id);

                var mostRecentExportDate = await _context.Orders.Where(order => order.Type == "Sale" &&
                                                                            order.Customer != null &&
                                                                            order.Customer.Company != null &&
                                                                            order.Customer.Company.CompanyId == companyId)
                                                            .OrderByDescending(order => order.Date)
                                                            .Select(order => order.Date)
                                                            .FirstOrDefaultAsync();

                var mostRecentImportDate = await _context.Orders.Where(order => order.Type == "Import" &&
                                                                                order.Customer != null &&
                                                                                order.Customer.Company != null &&
                                                                                order.Customer.Company.CompanyId == companyId)
                                                                 .OrderByDescending(order => order.Date)
                                                                .Select(order => order.Date)
                                                                .FirstOrDefaultAsync();
                var topExportOrders = await _context.Orders
                    .Where(order => order.Type == "Sale" &&
                                    order.Customer != null &&
                                    order.Customer.Company != null &&
                                    order.Date <= mostRecentExportDate &&
                                    order.Customer.Company.CompanyId == companyId)
                    .OrderByDescending(order => order.Date)
                    .Take(5)
                    .Select(o => new OrderDto
                    {
                        Id = o.OrderId,
                        Type = "Sale",
                        Status = o.Status.ToString(),
                        Date = o.Date,
                        TotalPrice = o.TotalPrice
                    })
                    .ToListAsync();

                var topImportOrders = await _context.Orders
                    .Where(order => order.Type == "Import" &&
                                    order.Customer != null &&
                                    order.Customer.Company != null &&
                                    order.Date <= mostRecentImportDate &&
                                    order.Customer.Company.CompanyId == companyId)
                    .OrderByDescending(order => order.Date)
                    .Take(5)
                    .Select(o => new OrderDto
                    {
                        Id = o.OrderId,
                        Type = "Import",
                        Status = o.Status.ToString(),
                        Date = o.Date,
                        TotalPrice = o.TotalPrice
                    })
                    .ToListAsync();
                if (topExportOrders.Any())
                {
                    rs.SaleOrders = topExportOrders;
                }
                if (topImportOrders.Any())
                {
                    rs.ImportOrder = topImportOrders;
                }
            }catch (Exception ex)
            {
                _loggerService.LogError("Error at get Top 5 order:  ", ex);
            }
        }
        return rs;
    }
}
