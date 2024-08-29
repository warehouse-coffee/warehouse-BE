using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace warehouse_BE.Infrastructure.permissions;

[Table("AspNetPermissions")]
public class Permission
{
    [Key]
    public int Id { get; set; } 

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty; // Tên của quyền, ví dụ: "ViewReports", "EditUsers"

    [MaxLength(250)]
    public string Description { get; set; } = string.Empty; // Mô tả về quyền này, có thể giúp hiểu rõ chức năng

    [Required]
    public string Module { get; set; } = string.Empty; // Module hoặc phần của hệ thống mà quyền này áp dụng, ví dụ: "UserManagement", "Reporting"

    public bool IsActive { get; set; } = true; 
}
