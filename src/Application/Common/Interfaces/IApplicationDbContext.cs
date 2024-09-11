using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<warehouse_BE.Domain.Entities.Product> Product { get; }

    DbSet<Company> Company { get; }
    DbSet<Order> Order { get; }
    DbSet<OrderDetail> OrderDetail { get; }

    DbSet<Storage> Storage { get; }
    DbSet<Area> Areas { get; }
    DbSet<Category> Category { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
