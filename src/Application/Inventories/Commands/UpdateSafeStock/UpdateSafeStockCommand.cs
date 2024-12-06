using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Inventories.Commands.UpdateSafeStock;

public class UpdateSafeStockCommand : IRequest<bool>
{
    public int InventoryId { get; set; } 
    public int SafeStock { get; set; }
}
public class UpdateSafeStockCommandHandler : IRequestHandler<UpdateSafeStockCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILoggerService _loggerService;

    public UpdateSafeStockCommandHandler(IApplicationDbContext context, ILoggerService loggerService)
    {
        _context = context;
        _loggerService = loggerService;
    }

    public async Task<bool> Handle(UpdateSafeStockCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.Id == request.InventoryId, cancellationToken);

            if (inventory == null)
            {
                return false;
            }

            inventory.SafeStock = request.SafeStock;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _loggerService.LogError("Error in UpdateSafeStockCommandHandler:", ex);
            return false;
        }
    }
}