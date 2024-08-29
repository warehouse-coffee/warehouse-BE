using Microsoft.AspNetCore.Identity;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public Company Company { get; set; } = null!;

    public List<Storage> storages { get; set; } = new List<Storage>();
}
