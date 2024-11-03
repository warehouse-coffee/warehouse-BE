using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Orders.Commands.DeleteOrder;

public class DeleteOrderCommand : IRequest<bool>
{
    public required string OrderId { get; set; }
}
public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
{
    private readonly IApplicationDbContext _context;
    public DeleteOrderCommandHandler(IApplicationDbContext context)
    {
       this._context = context;
    }
    public async Task<bool> Handle(DeleteOrderCommand request,CancellationToken cancellationToken)
    {
        int rs = 0;
        try
        {
            if(request.OrderId != null)
            {
                var orderr = await _context.Orders.Where(o => o.OrderId == request.OrderId).FirstOrDefaultAsync();
                if(orderr != null)
                {
                    orderr.IsDeleted = true;
                    rs = await _context.SaveChangesAsync(cancellationToken);
                }
                
            }
            return rs > 0;
        } catch 
        {
            return rs > 0;
        }
    }
}
