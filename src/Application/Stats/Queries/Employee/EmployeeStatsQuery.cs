using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Stats.Queries.Employee;

public class EmployeeStatsQuery : IRequest<EmployeeStatsVM>
{
}
public class EmployeeStatsQueryHandler : IRequestHandler<EmployeeStatsQuery, EmployeeStatsVM>
{
    private readonly IUser user;
    private readonly IApplicationDbContext context;
    private readonly IIdentityService identityService;
    private readonly ILoggerService loggerService;

    public EmployeeStatsQueryHandler(IUser user, IApplicationDbContext context, IIdentityService identityService, ILoggerService loggerService)
    {
        this.user = user;
        this.context = context;
        this.identityService = identityService;
        this.loggerService = loggerService;
    }   
    public async Task<EmployeeStatsVM> Handle(EmployeeStatsQuery request, CancellationToken cancellationToken)
    {
        var rs = new EmployeeStatsVM();
        int outboundInvent = 0;
        int producctExport = 0;
        int productImport = 0;
        int productExpiration = 0;
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        var currentDate = DateTime.Now;

        try
        {
            if (user.Id != null)
            {
                var (result, companyId) = await identityService.GetCompanyId(user.Id);
                var userStorages = await identityService.GetUserStoragesAsync(user.Id);
                // count total Product have quantity > 0 and Expiration 
                productExpiration = context.Products.AsQueryable()
                    .Where(p => p.Quantity > 0 && p.Expiration.Year == currentYear && p.Expiration < currentDate) 
                    .Count();
                // get list outbound  of inventory for the storage User management and the outbound with state complete and in the month 
                outboundInvent = context.InventoriesOutbound.AsQueryable()
                    .Where(io => io.CreatedBy == user.Id && 
                                 io.Status == OutboundStatus.Completed &&
                                 io.OutboundDate.HasValue &&
                                 io.OutboundDate.Value.Year == currentYear &&
                                 io.OutboundDate.Value.Month == currentMonth)
                    .Count();
                // count total Order with type Export in the month 
                var totalExportReservedQuantity = context.Orders
                    .Where(order => order.Type == "Export" && 
                                    order.Date.Year == currentYear && 
                                    order.Date.Month == currentMonth)
                    .SelectMany(order => order.Reservations) 
                    .Sum(reservation => reservation.ReservedQuantity);
                // count total Order with type Import in the month 
                var totalImportReservedQuantity = context.Orders
                    .Where(order => order.Type == "Import" && 
                                    order.Date.Year == currentYear && 
                                    order.Date.Month == currentMonth)
                    .SelectMany(order => order.Reservations) 
                    .Sum(reservation => reservation.ReservedQuantity); 

            }
        }
        catch (Exception ex)
        {
            loggerService.LogError("Error at Employee Stats Query: ", ex);
        }
        rs.TotalProductExportPerMonth = producctExport;
        rs.TotalProductImportPerMonth = productImport;
        rs.OutboundInventoryCompletePerMonth = outboundInvent;
        rs.ProductExpirationCount = productExpiration;
        return rs;
    }
}
