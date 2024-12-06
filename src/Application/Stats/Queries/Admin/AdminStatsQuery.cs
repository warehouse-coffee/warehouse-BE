using MediatR;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.SmartPriceService.Queries.GetCoffeePriceGraph;
using warehouse_BE.Domain.Enums;
using warehouse_BE.Domain.ValueObjects;

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
    private readonly IExternalHttpService externalHttpService;

    public AdminStatsQueryHandler(IUser user, IApplicationDbContext context, IIdentityService identityService, ILoggerService loggerService, IExternalHttpService externalHttpService)
    {
        this.user = user;
        this.context = context;
        this.identityService = identityService;
        this.loggerService = loggerService;
        this.externalHttpService = externalHttpService;
    }

    public async Task<AdminStatsVM> Handle (AdminStatsQuery request, CancellationToken cancellationToken)
    {
        
        var rs = new AdminStatsVM {
           HighDemandItemName = ""
        };
        var totalInventoryValue = 0m;
        var activeEmployeeCount = 0;
        double orderCompletionRate = 0;
        string highDemandItemName = "";
        int highDemandItemCount = 0;
        const string key = ConfigurationKeys.AiDriverServer;
        var prediction = new Prediction();
        try
        {
            if (user.Id != null)
            {
                var config = await context.Configurations
                  .FirstOrDefaultAsync(c => c.Key == key, cancellationToken);
                if (config != null)
                {
                    var baseUrl = config.Value;
                    var endpoint = $"{baseUrl}/predict_tommorow";

                    var ACURL = $"{baseUrl}/train_status";

                    var AIpredict = await externalHttpService.GetAsync<Prediction>(endpoint);

                    var AC = await externalHttpService.GetAsync<Prediction>(ACURL);
                    prediction.AI_predict = AIpredict?.AI_predict ?? 0;
                    prediction.Accuracy = AC?.Accuracy ?? 0;
                    prediction.Date = AC?.Date ?? DateTime.MinValue;

                }
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
                                              .Where(order => order.Customer != null && order.Customer.CompanyId == Cid && order.Status == OrderStatus.Completed && order.Type == "Sale" && !order.IsDeleted)
                                              .CountAsync();
                var orderCount = await context.Orders
                                              .Where(order => order.Customer != null && order.Customer.CompanyId == Cid && order.Type == "Sale" && !order.IsDeleted)
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
                    highDemandItemName =  $"{highDemandItem.ProductName}";
                    highDemandItemCount = highDemandItem.TotalReserved;
   
                }
            }
        } catch(Exception ex) 
        {
            loggerService.LogError("Error at AdminStastQuery: ", ex);
        }
        rs.TotalInventoryValue = totalInventoryValue;
        rs.OnlineEmployeeCount = activeEmployeeCount;
        rs.OrderCompletionRate = orderCompletionRate;
        rs.HighDemandItemName = highDemandItemName;
        rs.HighDemandItemCount = highDemandItemCount;
        rs.Prediction = prediction;
        return rs;
    }
}

