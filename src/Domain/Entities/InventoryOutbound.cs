namespace warehouse_BE.Domain.Entities;

public class InventoryOutbound : BaseAuditableEntity
{
    public DateTime? OutboundDate { get; set; } 
    public DateTime? ExpectedOutboundDate { get; set; } 
    public  string? OutboundBy { get; set; } 
    public OutboundStatus Status { get; set; } = OutboundStatus.Pending; 
    public string? Remarks { get; set; }

    // Relationships
    public required int OrderId { get; set; }
    public Order? Order { get; set; }
    public ICollection<OutboundDetail> OutboundDetails { get; set; } = new List<OutboundDetail>();
}

