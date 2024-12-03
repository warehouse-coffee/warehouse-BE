using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Principal;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Inventories.Commands.CountSafeStock;

public class CountSafeStockCommand : IRequest<int>
{
    public int Id { get; set; }
}
public class CountSafeStockCommandHandler : IRequestHandler<CountSafeStockCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;
    private  readonly ILoggerService _loggerService;
    public CountSafeStockCommandHandler(IApplicationDbContext context, IUser user, IIdentityService identityService, ILoggerService loggerService)
    {
        _context = context;
        _user = user;
        _identityService = identityService;
        _loggerService = loggerService;
    }
    public async Task<int> Handle(CountSafeStockCommand request, CancellationToken cancellationToken)
    {
        int rs = 0;
        if(_user.Id != null)
        {
            try
            {
                // get list storage
                var userStorages = await _identityService.GetUserStoragesAsync(_user.Id);
                var storageIds = userStorages.Select(s => s.Id).ToList();
                var Year = DateTime.Now.Year - 3;
                // order product amount 
                var orderproduct = await _context.Reservations.Include(r => r.Inventory)
                                    .Where( r => r.InventoryId == request.Id && r.Status == ReservationStatus.PickedUp 
                                    && !r.IsDeleted && storageIds.Contains(r.Inventory.StorageId) && r.ExpectedPickupDate.Year > Year)
                                    .Select(r => r.ReservedQuantity).SumAsync(cancellationToken);
                var safestock = orderproduct / 3;
                if (safestock > 0)
                {
                    rs += safestock;
                }
            } catch (Exception ex)
            {
                _loggerService.LogError("Error at CountSafeStockCommand: ", ex);
            }
        }
        return rs;
    }
}
