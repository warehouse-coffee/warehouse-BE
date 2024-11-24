using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Domain.Enums;
namespace warehouse_BE.Application.ReportStorage.Queries.GetReportOftheStorage;

public class GetReportOftheStorage : IRequest<ReportVM>
{
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
}
public class GetReportOftheStorageHandler : IRequestHandler<GetReportOftheStorage, ReportVM> 
{
    private readonly IUser _user;
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly ILoggerService _loggerService;
    
    public GetReportOftheStorageHandler(IUser user, IApplicationDbContext context, IIdentityService identityService, ILoggerService loggerService)
    {
        _user = user;
        _context = context;
        _identityService = identityService;
        _loggerService = loggerService;
    }
    public async Task<ReportVM> Handle(GetReportOftheStorage request,CancellationToken cancellationToken)
    {
        var WarehouseStatistics = new List<WarehousePerformance>();
        var ImportStatistics = new List<ImportSummary>();
        var TopProducts = new List<ProductPerformance>();
        var SlowMovingProducts = new List<ProductPerformance>();
        int totalOrder = 0;
        decimal totalRevenue = 0;
        decimal totalImportCost = 0;
        if (_user.Id != null)
        {
            try
            {
                var (result, userComapany) = await _identityService.GetCompanyId(_user.Id);

                var companyId = await _context.Companies.Where(o => !o.IsDeleted && o.CompanyId == userComapany).Select(o => o.Id).FirstOrDefaultAsync();
                var userStorages = await _identityService.GetUserStoragesAsync(_user.Id);
                var storageIds = userStorages.Select(o => o.Id).ToList();   
                if (userStorages.Any())
                {
                    // Lấy danh sách đơn hàng của công ty trong khoảng thời gian yêu cầu
                    totalRevenue = _context.Orders.Where(o => o.Customer != null &&
                                                            o.Customer.CompanyId == companyId &&
                                                            o.Status == Domain.Enums.OrderStatus.Completed &&
                                                            o.Date >= request.StartDate &&
                                                            o.Date <= request.EndDate &&
                                                            o.Type == "Sale").Sum(o => o.TotalPrice); 

                    totalImportCost = _context.Orders.Where(o => o.Customer != null &&
                                                                    o.Customer.CompanyId == companyId &&
                                                                    o.Date >= request.StartDate &&
                                                                    o.Date <= request.EndDate &&
                                                                    o.Type == "Import").Sum(o => o.TotalPrice); 

                    totalOrder =  _context.Orders.Where(o => o.Customer != null &&
                                                o.Customer.CompanyId == companyId &&
                                                o.Status == Domain.Enums.OrderStatus.Completed &&
                                                o.Date >= request.StartDate &&
                                                o.Date <= request.EndDate).Count();
                    TopProducts = _context.Products.Where(p => storageIds.Contains(p.StorageId) && (p.Status == ProductStatus.Available ||
                                                             p.Status == ProductStatus.Sold || p.Status == ProductStatus.Reserved)) 
                                                    .GroupBy(p => new { p.Name, p.StorageId })
                                                    .Select(g => new ProductPerformance
                                                    {
                                                        StorageId = g.Key.StorageId,
                                                        ProductName = g.Key.Name,
                                                        TotalSold = g.Sum(p => p.SoldQuantity),
                                                        AverageStorageTime = g.Average(p => (DateTime.UtcNow - p.ImportDate).TotalDays) 
                                                    })
                                                    .OrderByDescending(p => p.TotalSold)
                                                    .Take(10) 
                                                    .ToList();

                    SlowMovingProducts =  _context.Products.Where(p => (p.Status == ProductStatus.Available ||
                                                                   p.Status == ProductStatus.Sold || p.Status == ProductStatus.Reserved) 
                                                                   && storageIds.Contains(p.StorageId)) 
                                                           .GroupBy(p => new { p.Name, p.StorageId }) 
                                                           .Select(g => new ProductPerformance
                                                           {
                                                               StorageId = g.Key.StorageId,
                                                               ProductName = g.Key.Name, 
                                                               TotalSold = g.Sum(p => p.SoldQuantity), 
                                                               AverageStorageTime = g.Average(p => (DateTime.UtcNow - p.ImportDate).TotalDays) 
                                                           })
                                                           .OrderBy(p => p.TotalSold) 
                                                           .Take(10) 
                                                           .ToList();

                    var groupedOrders = _context.Orders.Where(o => o.Customer != null &&
                                                                        o.Customer.CompanyId == companyId &&
                                                                        o.Date >= request.StartDate &&
                                                                        o.Date <= request.EndDate &&
                                                                        o.Type == "Import") 
                                                            .GroupBy(o => o.CustomerId)
                                                            .ToList(); 

                    ImportStatistics = groupedOrders.Join(_context.Customers,
                                                          orderGroup => orderGroup.Key,
                                                          customer => customer.Id,
                                                          (orderGroup, customer) => new
                                                          {
                                                              orderGroup,
                                                              customer
                                                          })
                                                    .Select(result => new ImportSummary
                                                    {
                                                        SupplierName = result.customer.Name, 
                                                        TotalImportCost = result.orderGroup.Sum(o => o.TotalPrice) 
                                                    })
                                                    .OrderByDescending(s => s.TotalImportCost)
                                                    .Take(10)
                                                    .ToList();

                    var ordersData = await  _context.Orders.Where(o => o.Customer != null &&
                                                                        o.Customer.CompanyId == companyId &&
                                                                        o.Status == Domain.Enums.OrderStatus.Completed &&
                                                                        o.Date >= request.StartDate &&
                                                                        o.Date <= request.EndDate)
                                                            .SelectMany(o => o.Reservations) 
                                                            .Join(_context.Inventories, 
                                                                r => r.InventoryId, 
                                                                i => i.Id, 
                                                                (r, i) => new
                                                                {
                                                                    OrderId = r.Id,       
                                                                    ReservedQuantity = r.ReservedQuantity, 
                                                                    Price = r.Pricce,         
                                                                    InventoryId = r.InventoryId,   
                                                                    StorageId = i.StorageId,   
                                                                })
                                                            .ToListAsync(cancellationToken);
                    var inboundData = await _context.InventoriesOutbound.Where(io => io.Status == OutboundStatus.Completed && 
                                                                                       io.OutboundDate >= request.StartDate &&
                                                                                       io.OutboundDate <= request.EndDate)
                                                                           .SelectMany(io => io.OutboundDetails) 
                                                                           .Join(_context.OrderDetails, 
                                                                               od => od.Product != null ? od.Product.Id : (int?)null, 
                                                                               odt => odt.Product != null ? odt.Product.Id : (int?)null, 
                                                                               (od, odt) => new
                                                                               {
                                                                                   ProductId = od.Product != null ? od.Product.Id : (int?)null,
                                                                                   Quantity = od.Quantity,   
                                                                                   Price = odt.TotalPrice / odt.Quantity,
                                                                                   StorageId = od.Product != null ? od.Product.StorageId : (int?)null 
                                                                               })
                                                                           .Where(x => x.ProductId != null) 
                                                                           .GroupBy(x => x.ProductId) 
                                                                           .Select(g => new
                                                                           {
                                                                               ProductId = g.Key,
                                                                               TotalQuantity = g.Sum(x => x.Quantity), 
                                                                               Price =  g.First().Price, 
                                                                               StorageId = g.First().StorageId
                                                                           })
                                                                           .ToListAsync(cancellationToken);

                    var ordersGroupedByStorageId = ordersData.GroupBy(o => o.StorageId) 
                                                            .Select(g => new
                                                            {
                                                                StorageId = g.Key, 
                                                                TotalPrice = g.Sum(x => x.ReservedQuantity * x.Price) 
                                                            })
                                                            .ToList();
                    var inboundGroupedByStorageId = inboundData.GroupBy(i => i.StorageId)
                                                                .Select(g => new
                                                                {
                                                                    StorageId = g.Key, 
                                                                    TotalPrice = g.Sum(x => x.TotalQuantity * x.Price) 
                                                                })
                                                                .ToList();

                     WarehouseStatistics = ordersGroupedByStorageId.Join(inboundGroupedByStorageId, 
                                                                        o => o.StorageId,
                                                                        i => i.StorageId,
                                                                        (o, i) => new
                                                                        {
                                                                            StorageId = o.StorageId, 
                                                                            PriceRemaining = o.TotalPrice - i.TotalPrice 
                                                                        })
                                                                       .Join(_context.Storages, 
                                                                        result => result.StorageId,
                                                                        storage => storage.Id,
                                                                        (result, storage) => new WarehousePerformance
                                                                        {
                                                                            Id = result.StorageId,
                                                                            WarehouseName = storage.Name, 
                                                                            Revenue = Math.Round(result.PriceRemaining, 4) 
                                                                        })
                                                                    .ToList();
                }

                _loggerService.LogWarning("No storages found for the user." + _user.Id);
            }
            catch (Exception ex)
            {
                _loggerService.LogError("Error at GetReportOftheStorageHandler: ", ex);
            }
        }
        var rs = new ReportVM {
            TotalOrders = totalOrder,
            TotalImportCost = Math.Round(totalImportCost,4),
            TotalRevenue = Math.Round(totalRevenue),
            ImportStatistics = ImportStatistics,
            TopProducts = TopProducts,
            SlowMovingProducts = SlowMovingProducts,
            WarehouseStatistics= WarehouseStatistics,
        };

        return rs;
    }
}