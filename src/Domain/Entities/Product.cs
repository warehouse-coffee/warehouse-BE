namespace warehouse_BE.Domain.Entities;

public class Product : BaseAuditableEntity
{
    
    public required string Name { get; set; }
    public required string Units { get; set; }
    public int Quantity { get; set; }
    public string? Image { get; set; }
    public required ProductStatus Status { get; set; }  // 'Sold or Not Sold,...'
    public required DateTime Expiration { get; set; }
    public required DateTime ImportDate { get; set; }
    public  DateTime ExportDate { get; set; }
 

    // Relationships
    public required int CategoryId { get; set; }
    public required int AreaId { get; set; }
    public Category? Category { get; set; }
    public Area? Area { get; set; }
    public required int StorageId { get; set; }
    public Storage? Storage { get; set; }

}


