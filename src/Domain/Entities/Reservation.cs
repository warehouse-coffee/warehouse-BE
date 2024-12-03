namespace warehouse_BE.Domain.Entities;

public class Reservation : BaseAuditableEntity
{
    public int ReservedQuantity { get; set; } 
    public decimal Pricce { get; set; }
    public DateTime ReservedDate { get; set; }
    public DateTime ExpectedPickupDate { get; set; }

    public ReservationStatus Status { get; set; }

    //Relationships
    public required int InventoryId { get; set; }
    public required Inventory Inventory { get; set; }
}

