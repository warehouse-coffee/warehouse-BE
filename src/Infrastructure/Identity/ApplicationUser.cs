using Microsoft.AspNetCore.Identity;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? CompanyId { get; set; } = null!;

    public List<Storage> Storages { get; set; } = new List<Storage>();
}
