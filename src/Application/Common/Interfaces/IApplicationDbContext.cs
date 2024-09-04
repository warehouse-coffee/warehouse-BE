using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<warehouse_BE.Domain.Entities.Product> Product { get; }

    DbSet<Company> Company { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
