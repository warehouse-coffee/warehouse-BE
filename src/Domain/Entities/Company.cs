namespace warehouse_BE.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public string? CompanyName { get; set; }
    public int PhoneContact {  get; set; }
}
