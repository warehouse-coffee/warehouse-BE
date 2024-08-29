using System.ComponentModel.DataAnnotations.Schema;
using warehouse_BE.Infrastructure.Identity;

namespace warehouse_BE.Infrastructure.permissions;

public class RolePermissions
{
    [ForeignKey("AspNetRoles")]
    public string RoleId { get; set; } = string.Empty;
    public virtual ApplicationRole Role { get; set; } = new ApplicationRole();
    [ForeignKey("AspNetPermissions")]
    public int PermissionId { get; set; }
    public virtual Permission Permission { get; set; } = new Permission();
    public bool Deleted { get; set; }
}
