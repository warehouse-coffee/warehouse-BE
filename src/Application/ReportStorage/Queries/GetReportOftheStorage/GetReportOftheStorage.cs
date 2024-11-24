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
                    totalRevenue = _context.Orders
                                                .Where(o => o.Customer != null &&
                                                            o.Customer.CompanyId == companyId &&
                                                            o.Status == Domain.Enums.OrderStatus.Completed &&
                                                            o.Date >= request.StartDate &&
                                                            o.Date <= request.EndDate &&
                                                            o.Type == "Sale") // Chỉ tính đơn hàng loại "Sale"
                                                .Sum(o => o.TotalPrice); // Tổng doanh thu từ tất cả các đơn hàng

                    totalImportCost = _context.Orders
                                                        .Where(o => o.Customer != null &&
                                                                    o.Customer.CompanyId == companyId &&
                                                                    o.Date >= request.StartDate &&
                                                                    o.Date <= request.EndDate &&
                                                                    o.Type == "Import") // Chỉ tính đơn hàng loại "Import"
                                                        .Sum(o => o.TotalPrice); // Tổng chi phí nhập hàng từ tất cả các đơn hàng

                    totalOrder =  _context.Orders
                                    .Where(o => o.Customer != null &&
                                                o.Customer.CompanyId == companyId &&
                                                o.Status == Domain.Enums.OrderStatus.Completed &&
                                                o.Date >= request.StartDate &&
                                                o.Date <= request.EndDate)
                                    .Count();
                    TopProducts = _context.Products
                                        .Where(p => storageIds.Contains(p.StorageId) && (p.Status == ProductStatus.Available ||
                 p.Status == ProductStatus.Sold ||
                 p.Status == ProductStatus.Reserved)) // Giới hạn các sản phẩm đã bán
                                        .GroupBy(p => new { p.Name, p.StorageId }) // Nhóm theo Tên sản phẩm và StorageId
                                        .Select(g => new ProductPerformance
                                        {
                                            StorageId = g.Key.StorageId,
                                            ProductName = g.Key.Name, // Tên sản phẩm
                                            TotalSold = g.Sum(p => p.SoldQuantity), // Tổng số lượng bán ra
                                            AverageStorageTime = g.Average(p => (DateTime.UtcNow - p.ImportDate).TotalDays) // Thời gian lưu kho trung bình (tính từ ngày nhập hàng đến hiện tại)
                                        })
                                        .OrderByDescending(p => p.TotalSold) // Sắp xếp theo số lượng bán ra giảm dần
                                        .Take(10) // Lấy top 10 sản phẩm bán chạy nhất
                                        .ToList();

                    SlowMovingProducts =  _context.Products
                       .Where(p => (p.Status == ProductStatus.Available ||
                 p.Status == ProductStatus.Sold ||
                 p.Status == ProductStatus.Reserved) && storageIds.Contains(p.StorageId)) // Giới hạn các sản phẩm đã bán
                       .GroupBy(p => new { p.Name, p.StorageId }) // Nhóm theo Tên sản phẩm và StorageId
                       .Select(g => new ProductPerformance
                       {
                           StorageId = g.Key.StorageId,
                           ProductName = g.Key.Name, // Tên sản phẩm
                           TotalSold = g.Sum(p => p.SoldQuantity), // Tổng số lượng bán ra
                           AverageStorageTime = g.Average(p => (DateTime.UtcNow - p.ImportDate).TotalDays) // Thời gian lưu kho trung bình (tính từ ngày nhập hàng đến hiện tại)
                       })
                       .OrderBy(p => p.TotalSold) // Sắp xếp theo số lượng bán ra giảm dần
                       .Take(10) // Lấy top 10 sản phẩm bán chạy nhất
                       .ToList();

                    var groupedOrders = _context.Orders
                        .Where(o => o.Customer != null &&
                                    o.Customer.CompanyId == companyId &&
                                    o.Date >= request.StartDate &&
                                    o.Date <= request.EndDate &&
                                    o.Type == "Import") // Filter orders of type "Import"
                        .GroupBy(o => o.CustomerId)
                        .ToList(); // Get the grouped orders as a list

                    ImportStatistics = groupedOrders
                        .Join(_context.Customers,
                              orderGroup => orderGroup.Key,
                              customer => customer.Id,
                              (orderGroup, customer) => new
                              {
                                  orderGroup,
                                  customer
                              })
                        .Select(result => new ImportSummary
                        {
                            SupplierName = result.customer.Name, // Get the SupplierName from Customer entity
                            TotalImportCost = result.orderGroup.Sum(o => o.TotalPrice) // Sum of TotalPrice for each supplier
                        })
                        .OrderByDescending(s => s.TotalImportCost)
                        .Take(10)
                        .ToList();

                    var ordersData = await  _context.Orders
                                    .Where(o => o.Customer != null &&
                                                o.Customer.CompanyId == companyId &&
                                                o.Status == Domain.Enums.OrderStatus.Completed &&
                                                o.Date >= request.StartDate &&
                                                o.Date <= request.EndDate)
                                    .SelectMany(o => o.Reservations) // Lấy tất cả các Reservation liên quan đến Order
                                    .Join(_context.Inventories,  // Kết hợp với bảng Inventory để lấy StorageId và các thông tin cần thiết
                                        r => r.InventoryId, // Kết nối Reservation và Inventory qua InventoryId
                                        i => i.Id, // Kết nối InventoryId
                                        (r, i) => new
                                        {
                                            OrderId = r.Id,       // Lấy OrderId từ Reservation
                                            ReservedQuantity = r.ReservedQuantity,  // Lấy ReservedQuantity từ Reservation
                                            Price = r.Pricce,           // Lấy Price từ Reservation
                                            InventoryId = r.InventoryId,   // Lấy InventoryId từ Reservation
                                            StorageId = i.StorageId,    // Lấy StorageId từ Inventory
                                        })
                                    .ToListAsync(cancellationToken);
                    var inboundData = await _context.InventoriesOutbound
                                                       .Where(io => io.Status == OutboundStatus.Completed && // Chỉ lấy các Outbound đã hoàn thành
                                                                   io.OutboundDate >= request.StartDate &&
                                                                   io.OutboundDate <= request.EndDate)
                                                       .SelectMany(io => io.OutboundDetails) // Lấy tất cả các OutboundDetail liên quan đến InventoryOutbound
                                                       .Join(_context.OrderDetails, // Kết hợp với OrderDetails để lấy giá của sản phẩm
                                                           od => od.Product != null ? od.Product.Id : (int?)null, // Kiểm tra null và lấy ProductId nếu Product không null
                                                           odt => odt.Product != null ? odt.Product.Id : (int?)null, // Kết nối với OrderDetail qua ProductId
                                                           (od, odt) => new
                                                           {
                                                               ProductId = od.Product != null ? od.Product.Id : (int?)null, // Kiểm tra null và lấy ProductId nếu Product không null
                                                               Quantity = od.Quantity,    // Lấy Quantity từ OutboundDetail
                                                               Price = odt.TotalPrice / odt.Quantity, // Tính giá đơn vị từ OrderDetail
                                                               StorageId = od.Product != null ? od.Product.StorageId : (int?)null // Kiểm tra null và lấy StorageId từ Product nếu không null
                                                           })
                                                       .Where(x => x.ProductId != null) // Loại bỏ các kết quả có ProductId là null
                                                       .GroupBy(x => x.ProductId) // Nhóm theo ProductId để tính tổng giá trị của mỗi sản phẩm
                                                       .Select(g => new
                                                       {
                                                           ProductId = g.Key, // Lấy ProductId
                                                           TotalQuantity = g.Sum(x => x.Quantity), // Tổng số lượng sản phẩm
                                                           Price =  g.First().Price, 
                                                           StorageId = g.First().StorageId // Lấy StorageId từ sản phẩm đầu tiên trong nhóm (vì StorageId là giống nhau cho mỗi sản phẩm)
                                                       })
                                                       .ToListAsync(cancellationToken);

                    var ordersGroupedByStorageId = ordersData
                                                        .GroupBy(o => o.StorageId) // Nhóm theo StorageId
                                                        .Select(g => new
                                                        {
                                                            StorageId = g.Key, // Lấy StorageId
                                                            TotalPrice = g.Sum(x => x.ReservedQuantity * x.Price) // Tính tổng giá trị của mỗi StorageId
                                                        })
                                                        .ToList();
                    var inboundGroupedByStorageId = inboundData
                                                            .GroupBy(i => i.StorageId) // Nhóm theo StorageId
                                                            .Select(g => new
                                                            {
                                                                StorageId = g.Key, // Lấy StorageId
                                                                TotalPrice = g.Sum(x => x.TotalQuantity * x.Price) // Tính tổng giá trị của mỗi StorageId
                                                            })
                                                            .ToList();

                     WarehouseStatistics = ordersGroupedByStorageId
                                        .Join(inboundGroupedByStorageId, // Join ordersData and inboundData by StorageId
                                            o => o.StorageId,
                                            i => i.StorageId,
                                            (o, i) => new
                                            {
                                                StorageId = o.StorageId, // StorageId from orders
                                                PriceRemaining = o.TotalPrice - i.TotalPrice // Calculate remaining price
                                            })
                                           .Join(_context.Storages, // Join with storages to get WarehouseName (assuming storages is a collection of Storage entities)
                                            result => result.StorageId,
                                            storage => storage.Id, // Assuming StorageId corresponds to Storage.Id
                                            (result, storage) => new WarehousePerformance
                                            {
                                                Id = result.StorageId, // Mapping StorageId to Id
                                                WarehouseName = storage.Name, // Mapping warehouse name from Storage
                                                Revenue = Math.Round(result.PriceRemaining, 4) // Round revenue to 4 decimal places
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