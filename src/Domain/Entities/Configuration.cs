using System.ComponentModel.DataAnnotations;

namespace warehouse_BE.Domain.Entities;

public class Configuration : BaseAuditableEntity
{
    [Required]
    [StringLength(100)]
    public required string Key { get; set; } 
    [Required]
    public required string Value { get; set; } 
}
