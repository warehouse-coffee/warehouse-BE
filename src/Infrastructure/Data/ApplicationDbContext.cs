using System.Reflection;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Domain.Entities;
using warehouse_BE.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace warehouse_BE.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Product => Set<Product>();
    public DbSet<Company> Company { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>()
           .HasOne<Company>() // Chỉ định kiểu đối tượng liên kết là Company
           .WithMany() // Chỉ định rằng Company không có thuộc tính điều hướng đến ApplicationUser
           .HasForeignKey(u => u.CompanyId) // Chỉ định thuộc tính khóa ngoại trong ApplicationUser
           .HasPrincipalKey(c => c.CompanyId) // Chỉ định thuộc tính khóa chính trong Company
           .OnDelete(DeleteBehavior.SetNull);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
