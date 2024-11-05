using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }


    public int? CompanyId { get; set; }
    public Company? Company { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
