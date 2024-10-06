using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Orders.Commands.ImportStorage;

public class ImportStogareCommand : IRequest<ResponseDto>
{
    public required string Type { get; set; }
    public decimal TotalPrice { get; set; }
    public List<ImportProductDto>? Products { get; set; }
}
public class ImportStogareCommandHandler : IRequestHandler<ImportStogareCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IUser currentUser;

    public ImportStogareCommandHandler(IApplicationDbContext context, IIdentityService identityService, IUser currentUser)
    {
        _context = context;
        _identityService = identityService;
        this.currentUser = currentUser;
    }
    public async Task<ResponseDto> Handle(ImportStogareCommand request, CancellationToken cancellationToken)
    {
        var rs = new ResponseDto();

        if (currentUser.Id == null)
        {
            rs.StatusCode = 401;
            rs.Message = "Invalid or unauthorized user.";
            return rs;
        }

        if (request.Products == null || !request.Products.Any())
        {
            rs.StatusCode = 400;
            rs.Message = "Product list cannot be empty.";
            return rs;
        }
        // Check if CategoryId, AreaId, and StorageId exist
        var categoryIds = request.Products.Select(p => p.CategoryId).Distinct().ToList();
        var areaIds = request.Products.Select(p => p.AreaId).Distinct().ToList();
        var storageIds = request.Products.Select(p => p.StorageId).Distinct().ToList();

        // Check categories
        var categoriesExist = await _context.Categories
            .Where(c => categoryIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(cancellationToken);

        if (categoriesExist.Count != categoryIds.Count)
        {
            rs.StatusCode = 400;
            rs.Message = "One or more CategoryIds are invalid.";
            return rs;
        }

        // Check areas
        var areasExist = await _context.Areas
            .Where(a => areaIds.Contains(a.Id))
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        if (areasExist.Count != areaIds.Count)
        {
            rs.StatusCode = 400;
            rs.Message = "One or more AreaIds are invalid.";
            return rs;
        }

        // Check storages
        var storagesExist = await _context.Storages
            .Where(s => storageIds.Contains(s.Id))
            .Select(s => s.Id)
            .ToListAsync(cancellationToken);

        if (storagesExist.Count != storageIds.Count)
        {
            rs.StatusCode = 400;
            rs.Message = "One or more StorageIds are invalid.";
            return rs;
        }


        var calculatedTotalPrice = request.Products.Sum(p => p.Price * p.Quantity);
        if (request.TotalPrice != calculatedTotalPrice)
        {
            rs.StatusCode = 400;
            rs.Message = "TotalPrice does not match the total price of the products.";
            return rs;
        }
        try
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                Type = request.Type,
                Date = DateTime.UtcNow,
                TotalPrice = request.TotalPrice
            };

            foreach (var productDto in request.Products)
            {

                var product = new Product
                {
                    Name = productDto.Name!,
                    Units = productDto.Unit,
                    Amount = productDto.Quantity,
                    Status = Domain.Enums.ProductStatus.Available,
                    Expiration = productDto.Expiration,
                    ImportDate = DateTime.UtcNow,
                    CategoryId = productDto.CategoryId,
                    AreaId = productDto.AreaId,
                    StorageId = productDto.StorageId
                };
                _context.Products.Add(product);

                var orderDetail = new OrderDetail
                {
                    Quantity = productDto.Quantity,
                    TotalPrice = productDto.Price * productDto.Quantity,
                    Note = productDto.Note,
                };

                orderDetail.Products.Add(product);
                order.OrderDetails.Add(orderDetail);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            rs.StatusCode = 400;
            rs.Message = "Product import unsuccessful.";
        }
        rs.StatusCode = 201;
        rs.Message = "Product import successful.";

        return rs;
    }
}
