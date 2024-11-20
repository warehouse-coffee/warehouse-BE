using warehouse_BE.Domain.Enums;

namespace warehouse_BE.Application.Orders.Queries.GetTopFiveSaleAndImportOrder;

public class OrderDto
{
    public string? Id { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public DateTime Date { get; set; }

    public decimal TotalPrice { get; set; }
}
