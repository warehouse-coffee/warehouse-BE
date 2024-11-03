namespace warehouse_BE.Domain.Entities;

public class Reservation : BaseAuditableEntity
{
    public required int InventoryId { get; set; }
    public required Inventory Inventory { get; set; }

    public int ReservedQuantity { get; set; } 
    public DateTime ReservedDate { get; set; }
    public DateTime? ExpectedPickupDate { get; set; }

    public ReservationStatus Status { get; set; }

    //Relationships
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
}

