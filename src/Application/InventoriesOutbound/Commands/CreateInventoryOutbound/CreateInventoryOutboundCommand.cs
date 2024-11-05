using Microsoft.AspNetCore.Builder;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.InventoriesOutbound.Commands.CreateInventoryOutbound;

public class CreateInventoryOutboundCommand : IRequest<ResponseDto>
{
    public int OrderId { get; set; }
}
public class CreateInventoryOutboundCommandHandler : IRequestHandler<CreateInventoryOutboundCommand, ResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _currentUser;
    private readonly IMapper _mapper;
    private readonly ILoggerService _loggerService;

    public CreateInventoryOutboundCommandHandler(IApplicationDbContext context , IUser currentUser, IMapper mapper, ILoggerService loggerService)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
        _loggerService = loggerService;
    }

    public async Task<ResponseDto> Handle(CreateInventoryOutboundCommand request, CancellationToken cancellationToken)
    {
        var response = new ResponseDto();
        if (request.OrderId <= 0)
        {
            response.StatusCode = 400;
            response.Message = "Invalid Order ID.";
            return response;
        }

        try
        {
            var existingOutbound = await _context.InventoriesOutbound
          .FirstOrDefaultAsync(io => io.OrderId == request.OrderId && !io.IsDeleted, cancellationToken);

            if (existingOutbound != null)
            {
                response.StatusCode = 400;
                response.Message = "An inventory outbound already exists for this Order ID.";
                return response;
            }
            var order = await _context.Orders
                .Include(o => o.Reservations)
                .ThenInclude(r => r.Inventory)
                .ThenInclude(i => i.Products) // Include Products from Inventory
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
            {
                response.StatusCode = 400;
                response.Message = "Order not found.";
                return response;
            }

            var reservations = order.Reservations.Where(r => r.Status == ReservationStatus.Pending).ToList();

            if (!reservations.Any())
            {
                response.StatusCode = 400;
                response.Message = "No reservations found for this order.";
                return response;
            }

            var outboundDetails = new List<OutboundDetail>();

            foreach (var reservation in reservations)
            {
                var inventory = reservation.Inventory;
                if (inventory == null)
                {
                    response.StatusCode = 400;
                    response.Message = "Inventory not found for this reservation.";
                    return response;
                }

                // Check if there are any products in the inventory
                if (!inventory.Products.Any())
                {
                    response.StatusCode = 400;
                    response.Message = "No products available in inventory.";
                    return response;
                }

                // Iterate through the products to check available quantity
                int remainingQuantity = reservation.ReservedQuantity;

                foreach (var product in inventory.Products)
                {
                    if (product.Quantity <= 0)
                    {
                        continue; // Skip products with no available quantity
                    }

                    // Determine how much we can allocate from this product
                    int quantityToAllocate = Math.Min(remainingQuantity, product.Quantity);

                    // Create outbound detail
                    var outboundDetail = new OutboundDetail
                    {
                        ProductId = product.Id, // Use Product ID
                        Quantity = quantityToAllocate,
                        IsAvailable = true, // Assuming product is available after allocation
                        Note = $"Outbound for Order {request.OrderId}",
                        InventoryOutboundId = 0 // Set this later when creating InventoryOutbound
                    };

                    // Update inventory quantities
                    product.Quantity -= quantityToAllocate; // Update the product's total quantity
                    remainingQuantity -= quantityToAllocate; // Reduce the remaining quantity we need

                    outboundDetails.Add(outboundDetail);

                    // Break if we have fulfilled the reservation
                    if (remainingQuantity <= 0)
                    {
                        break;
                    }
                }

                // After attempting to allocate from all products, check if we still have remaining quantity
                if (remainingQuantity > 0)
                {
                    var productName = inventory.Products.FirstOrDefault()?.Name ?? "Unknown Product";
                    response.StatusCode = 400;
                    response.Message = $"Not enough available stock for product '{productName}' to fulfill the reservation of {reservation.ReservedQuantity}.";
                    return response;
                }
            }
        

            // Create an InventoryOutbound object
            var inventoryOutbound = new InventoryOutbound
            {
                OutboundDate = DateTime.UtcNow,
                OutboundBy = _currentUser.Id,
                Remarks = $"Outbound created for Order ID: {request.OrderId}",
                Status = OutboundStatus.Pending,
                OrderId = order.Id,
                OutboundDetails = outboundDetails
            };

            // Save to the database
            _context.InventoriesOutbound.Add(inventoryOutbound);
            await _context.SaveChangesAsync(cancellationToken);

            var mappedOutbound = _mapper.Map<InventoryOutboundVM>(inventoryOutbound);

            response.StatusCode = 201;
            response.Message = "Outbound created successfully.";
            response.Data = mappedOutbound; 
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Error at Create Inventories Outbound: ", ex);
            response.StatusCode = 500; // Internal server error
            response.Message = "An error occurred while creating the inventory outbound.";
        }

        return response;
    }

}
