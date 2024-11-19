using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

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
        try
        {
            if (user.Id != null)
            {
                var (result, companyId) = await identityService.GetCompanyId(user.Id);
                var userStorages = await identityService.GetUserStoragesAsync(user.Id);
                // count total Product have quantity > 0 and Expiration 
                var queryProductExpiration = context.Products.AsQueryable();

                // get list outbound  of inventory for the storage User management and the outbound with state complete and in the month 
                var queryOutboundInvent = context.InventoriesOutbound.AsQueryable().Where(o => o.CreatedBy == user.Id);


                // count total Order with type Export in the month 
                var queryproductExport = context.Orders.AsQueryable();


                // count total Order with type Import in the month 
                var queryproductImport = context.Orders.AsQueryable();

            }
        }catch (Exception ex)
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
