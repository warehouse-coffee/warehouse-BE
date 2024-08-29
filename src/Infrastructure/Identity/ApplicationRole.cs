
using Microsoft.AspNetCore.Identity;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Infrastructure.permissions;

namespace warehouse_BE.Infrastructure.Identity;

public class ApplicationRole : IdentityRole
{
    public virtual ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();

    public int CompanyId { get; set; }
    public Company Company { get; set; } = new Company();
}
