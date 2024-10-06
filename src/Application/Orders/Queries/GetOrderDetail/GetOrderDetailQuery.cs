

using System.Reflection.Metadata;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Orders.Queries.GetOrderDetail;

public class GetOrderDetailQuery : IRequest<OrderDetailVM>
{
    public required string OrderId { get; set; }
    
}
public class GetOrderDetailQueryHandler : IRequestHandler<GetOrderDetailQuery, OrderDetailVM>
{
    public readonly IApplicationDbContext _context;
    public readonly IMapper _mapper;
    public GetOrderDetailQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<OrderDetailVM> Handle(GetOrderDetailQuery request,CancellationToken cancellationToken)
    {
        var rs = new OrderDetailVM();
        try {
            var order = await _context.Orders
             .Where(o => o.OrderId == request.OrderId)
             .ProjectTo<OrderDetailVM>(_mapper.ConfigurationProvider)
             .AsSplitQuery() 
             .FirstOrDefaultAsync(cancellationToken);

            if ( order != null )
            {
                rs = order;
            }
        }
        catch {
            
        }
        return rs;
    }
}