namespace warehouse_BE.Domain.Entities;

public class OutboundDetail : BaseAuditableEntity
{
    public required int ProductId { get; set; }
    public Product? Product { get; set; }

    public required int Quantity { get; set; }
    public bool IsAvailable { get; set; }
    public string? Note { get; set; }

    // relationship
    public required int InventoryOutboundId { get; set; }
    public InventoryOutbound? InventoryOutbound { get; set; }
}

