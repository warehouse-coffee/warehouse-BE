namespace warehouse_BE.Domain.Entities;
public class UserStorage : BaseAuditableEntity
{
    public required string UserId { get; set; }

    public int StorageId { get; set; }
    public Storage Storage { get; set; } = null!;
}
