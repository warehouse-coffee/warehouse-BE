using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Orders.Commands.SaleOrder;

public class SaleOrderCommand : IRequest<ResponseDto>
{
    public decimal TotalPrice { get; set; }
    public int CustomerId { get; set; }
    public List<SaleOrderProduct> Products { get; set; } = new List<SaleOrderProduct>();

}
public class SaleOrderCommandHandler : IRequestHandler<SaleOrderCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public SaleOrderCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ResponseDto> Handle(SaleOrderCommand request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto();

        if (request.CustomerId == 0)
        {
            response.StatusCode = 400;
            response.Message = "Customer ID cannot be zero or empty.";
            return response;
        }

        var customer = await _context.Customers
            .Where(o => o.Id == request.CustomerId && !o.IsDeleted)
            .FirstOrDefaultAsync();

        if (customer == null)
        {
            response.StatusCode = 404;
            response.Message = "Customer not found.";
            return response;
        }
        if (request.Products == null || !request.Products.Any())
        {
            response.StatusCode = 400;
            response.Message = "Product list cannot be empty.";
            return response;
        }

        var calculatedTotalPrice = request.Products.Sum(p => p.Price * p.Quantity);
        if (request.TotalPrice != calculatedTotalPrice)
        {
            response.StatusCode = 400;
            response.Message = "Total price does not match the sum of product prices.";
            return response;
        }

        try
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                Type = "Sale",
                Date = DateTime.UtcNow,
                TotalPrice = request.TotalPrice,
                Status = OrderStatus.Pending,
                CustomerId = customer.Id
            };

            foreach (var productDto in request.Products)
            {
                int remainingQuantity = productDto.Quantity;
                var inventories = await _context.Inventories
                 .Where(i => i.ProductName == productDto.ProductName && (i.TotalQuantity - i.ReservedQuantity) > 0)
                 .OrderByDescending(i => i.TotalQuantity - i.ReservedQuantity)
                 .ToListAsync(cancellationToken);


                if (!inventories.Any())
                {
                    response.StatusCode = 400;
                    response.Message = $"Insufficient stock for product '{productDto.ProductName}'.";
                    return response;
                }

                foreach (var inventory in inventories)
                {
                    if (remainingQuantity <= 0) break;

                    int allocatedQuantity = Math.Min(remainingQuantity, inventory.AvailableQuantity);

                    var reservation = new Reservation
                    {
                        ReservedQuantity = allocatedQuantity,
                        ReservedDate = DateTime.UtcNow,
                        ExpectedPickupDate = productDto.ExpectedPickupDate,
                        Status = ReservationStatus.Pending,
                        InventoryId = inventory.Id,
                        Inventory = inventory
                    };

                    order.Reservations.Add(reservation);

                    remainingQuantity -= allocatedQuantity;
                    inventory.ReservedQuantity += allocatedQuantity; 
                }

                if (remainingQuantity > 0)
                {
                    response.StatusCode = 400;
                    response.Message = $"Unable to fulfill the quantity requested for product '{productDto.ProductName}'.";
                    return response;
                }
            }
        

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            var data = _mapper.Map<SaleOrderVM>(order);

            response.StatusCode = 201;
            response.Message = "Sale order created successfully with reservations.";
            response.Data = data;
        }
        catch
        {
            response.StatusCode = 400;
            response.Message = "Sale order creation failed.";
        }

        return response;
    }
}
