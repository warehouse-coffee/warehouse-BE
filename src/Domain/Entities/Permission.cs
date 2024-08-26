using System.ComponentModel.DataAnnotations.Schema;

namespace warehouse_BE.Domain.Entities;

public class Permission : BaseAuditableEntity
{
    // Relationships
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    [ForeignKey("StorageId")]
    public Storage Storage { get; set; } = null!;
}

