namespace warehouse_BE.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public required string CompanyId { get; set; }
    public required string CompanyName { get; set; }
    public required string PhoneContact {  get; set; }

    public required string EmailContact { get; set; }
    public string? Address { get; set; }
}
