namespace warehouse_BE.Domain.Entities;

public class Storage : BaseAuditableEntity
{
    public string? Name { get; set; }
    public string? Location { get; set; }
    public int Status { get; set; }  // 'Active or Not Active'

    // Relationships
    public ICollection<Area>? Areas { get; set; }
}
