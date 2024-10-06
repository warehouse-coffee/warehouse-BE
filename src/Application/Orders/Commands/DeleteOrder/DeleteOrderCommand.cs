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
        var rs = false;
        try
        {
            if(request.OrderId != null)
            {
                var orderr = await _context.Orders.Where(o => o.OrderId == request.OrderId).FirstOrDefaultAsync();
                if(orderr != null)
                {
                    var result =  _context.Orders.Remove(orderr);
                    await _context.SaveChangesAsync(cancellationToken);
                    rs = true;
                }
                
            }
            return rs;
        } catch 
        {
            return rs;
        }
    }
}
