using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Customers.Commands.CreateCustomer;
using warehouse_BE.Application.Orders.Queries.GetOrderList;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Orders.Commands.ImportStorage;

public class ImportStogareCommand : IRequest<ResponseDto>
{
    public decimal TotalPrice { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhoneNumber { get; set; }
    public List<ImportProductDto>? Products { get; set; }
}
public class ImportStogareCommandHandler : IRequestHandler<ImportStogareCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;
    private readonly IMapper _mapper;

    public ImportStogareCommandHandler(IApplicationDbContext context, IIdentityService identityService, IUser currentUser, IMapper mapper)
    {
        _context = context;
        _identityService = identityService;
        _currentUser = currentUser;
        _mapper = mapper;
    }
    public async Task<ResponseDto> Handle(ImportStogareCommand request, CancellationToken cancellationToken)
    {
        var rs = new ResponseDto();

        if (_currentUser.Id == null)
        {
            rs.StatusCode = 401;
            rs.Message = "Invalid or unauthorized user.";
            return rs;
        }
        if(string.IsNullOrEmpty(request.CustomerName) && string.IsNullOrEmpty(request.CustomerPhoneNumber))
        {
            rs.StatusCode = 400;
            rs.Message = "Customer is required.";
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
            var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Name == request.CustomerName && c.Phone == request.CustomerPhoneNumber, cancellationToken);

            if (customer == null)
            {
                 customer = new Customer
                {
                    Name = request.CustomerName!,
                    Phone = request.CustomerPhoneNumber
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                Type = "Import",
                Date = DateTime.UtcNow,
                TotalPrice = request.TotalPrice,
                Status = OrderStatus.Pending,
                CustomerId = customer.Id,
            };
            var inventories = new Dictionary<string, Inventory>();
            foreach (var productDto in request.Products)
            {
                var key = $"{productDto.Name}-{productDto.StorageId}";
                var inventory = await _context.Inventories
                        .FirstOrDefaultAsync(i =>
                            i.ProductName == productDto.Name &&
                            i.StorageId == productDto.StorageId,
                            cancellationToken);
                if (productDto.Expiration < DateTime.UtcNow)
                {
                    rs.StatusCode = 400;
                    rs.Message = $"Product '{productDto.Name}' has expired and cannot be imported.";
                    return rs;
                }
                if (!inventories.ContainsKey(key))
                {
                    if (inventory == null)
                    {
                        inventory = new Inventory
                        {
                            ProductName = productDto.Name,
                            TotalQuantity = productDto.Quantity,
                            TotalPrice = productDto.Price * productDto.Quantity,
                            Expiration = productDto.Expiration,
                            CategoryId = productDto.CategoryId,
                            StorageId = productDto.StorageId,
                        };
                        _context.Inventories.Add(inventory);
                        inventories[key] = inventory;
                    }
                    else
                    {
                        inventory.TotalQuantity += productDto.Quantity;
                        inventory.TotalPrice += productDto.Price * productDto.Quantity;

                        if (inventory.Expiration == null || productDto.Expiration < inventory.Expiration)
                        {
                            inventory.Expiration = productDto.Expiration;
                        }
                        inventories[key] = inventory;
                    }
                }
                else
                {
                    inventory = inventories[key];
                   
                    inventory.TotalQuantity += productDto.Quantity;
                    inventory.TotalPrice += productDto.Price * productDto.Quantity;

                    if (inventory.Expiration == null || productDto.Expiration < inventory.Expiration)
                    {
                        inventory.Expiration = productDto.Expiration;
                    }
                }

                var product = new Product
                {
                    Name = productDto.Name!,
                    Units = productDto.Unit,
                    Quantity = productDto.Quantity,
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
                    Product = product
                };
                order.OrderDetails.Add(orderDetail);
                inventory.Products.Add(product);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            // Map Order to OrderDto to return the newly created data
            var data = _mapper.Map<OrderDto>(order);

            rs.StatusCode = 201;
            rs.Message = "Product import successful.";
            rs.Data = data;
        }
        catch
        {
            rs.StatusCode = 400;
            rs.Message = "Product import unsuccessful.";
        }

        return rs;
    }
}
