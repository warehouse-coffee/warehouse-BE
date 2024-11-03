using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Company> Companies { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderDetail> OrderDetails { get; }

    DbSet<Storage> Storages { get; }
    DbSet<Area> Areas { get; }
    DbSet<Category> Categories { get; }
    DbSet<Configuration> Configurations { get; }
    DbSet<Inventory> Inventories { get; }
    DbSet<Reservation> Reservations { get; } 
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
