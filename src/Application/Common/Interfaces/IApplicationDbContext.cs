using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    //DbSet<TodoList> TodoLists { get; }

    //DbSet<TodoItem> TodoItems { get; }

    DbSet<warehouse_BE.Domain.Entities.Product> Product { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
