namespace warehouse_BE.Domain.Entities;

public class Order : BaseAuditableEntity
{
    public required string OrderId { get; set; }
    public required string Type { get; set; }

    public required DateTime Date { get; set; } 

    public required decimal TotalPrice { get; set; } 

    // Relationships
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public int? ReservationId { get; set; }
    public Reservation? Reservation { get; set; }
}
