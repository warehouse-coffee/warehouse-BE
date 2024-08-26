using Microsoft.AspNetCore.Identity;

namespace warehouse_BE.Domain.Entities;

public class User : IdentityUser
{
    // Relationships
    public Company Company { get; set; } = null!;
}
