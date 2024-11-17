using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Stats.Queries.Admin;

public class AdminStatsQuery : IRequest<AdminStatsVM>
{

}
public class AdminStatsQueryHandler : IRequestHandler<AdminStatsQuery, AdminStatsVM> 
{ 
    private readonly IUser user;
    private readonly IApplicationDbContext context; 
    private readonly IIdentityService identityService;  
    private readonly ILoggerService loggerService;

    public AdminStatsQueryHandler(IUser user, IApplicationDbContext context, IIdentityService identityService, ILoggerService loggerService)
    {
        this.user = user;
        this.context = context;
        this.identityService = identityService;
        this.loggerService = loggerService;
    }

    public async Task<AdminStatsVM> Handle (AdminStatsQuery request, CancellationToken cancellationToken)
    {
        
        var rs = new AdminStatsVM {
           HighDemandItemSummary = ""
        };
        var totalInventoryValue = 0m;
        var activeEmployeeCount = 0;
        double orderCompletionRate = 0;
        string highDemandItemSummary = "";

        try
        {
            if (user.Id != null)
            {
                var (result, companyId) = await identityService.GetCompanyId(user.Id);
                var userStorages = await identityService.GetUserStoragesAsync(user.Id);
                var storageIds = userStorages.Select(s => s.Id).ToList();
                var data = context.Inventories
                    .Where(o => storageIds.Contains(o.StorageId)) 
                    .Select(o => o.TotalPrice) 
                    .ToList();
                totalInventoryValue = data.Sum();

                activeEmployeeCount = await identityService.TotalOnlineEmployee(companyId);
                var company = await context.Companies
                       .FirstOrDefaultAsync(c => c.CompanyId == companyId, cancellationToken);
                int Cid = -1;
                if(company != null)
                {
                    Cid = company.Id;
                }
                var completedOrdersCount = await context.Orders
                                              .Where(order => order.Customer != null && order.Customer.CompanyId == Cid && order.Status == OrderStatus.Completed && order.Type == "Sale")
                                              .CountAsync();
                var orderCount = await context.Orders
                                              .Where(order => order.Customer != null && order.Customer.CompanyId == Cid && order.Type == "Sale")
                                              .CountAsync();
                orderCompletionRate = orderCount > 0 ? (double)completedOrdersCount / orderCount * 100 : 0;
                orderCompletionRate = Math.Round(orderCompletionRate, 2);
                var highDemandItem = await context.Reservations.AsQueryable()
                                                              .Where(r => r.Inventory != null && r.Inventory.Storage != null && storageIds.Contains(r.Inventory.Storage.Id))
                                                              .GroupBy(r => r.Inventory.ProductName)
                                                              .Select(g => new
                                                              {
                                                                  ProductName = g.Key,
                                                                  TotalReserved = g.Sum(r => r.ReservedQuantity)
                                                              })
                                                              .OrderByDescending(x => x.TotalReserved)
                                                              .FirstOrDefaultAsync();
                if (highDemandItem != null)
                {
                    highDemandItemSummary =  $"{highDemandItem.ProductName} - {highDemandItem.TotalReserved} units ordered";
                }
            }
        } catch(Exception ex) 
        {
            loggerService.LogError("Error at AdminStastQuery: ", ex);
        }
        rs.TotalInventoryValue = totalInventoryValue;
        rs.OnlineEmployeeCount = activeEmployeeCount;
        rs.OrderCompletionRate = orderCompletionRate;
        rs.HighDemandItemSummary = highDemandItemSummary;
        return rs;
    }
}

