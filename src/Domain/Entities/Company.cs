namespace warehouse_BE.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public  string? CompanyId { get; set; }
    public  string? CompanyName { get; set; }
    public  string? PhoneContact {  get; set; }

    public  string? EmailContact { get; set; }
    public string? Address { get; set; }
}
