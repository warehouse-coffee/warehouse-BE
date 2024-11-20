
namespace warehouse_BE.Application.Orders.Queries.GetTopFiveSaleAndImportOrder;

public class SaleAndImportOrderVM
{
    public SaleAndImportOrderVM()
    {
        SaleOrders = new List<OrderDto>();
        ImportOrder = new List<OrderDto>();
    }
    public List<OrderDto>? SaleOrders { get; set; }
    public List<OrderDto>? ImportOrder { get; set; }
}
