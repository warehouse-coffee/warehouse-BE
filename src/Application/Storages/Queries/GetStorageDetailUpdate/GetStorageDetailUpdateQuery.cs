using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Storages.Commads.UpdateStorage;

namespace warehouse_BE.Application.Storages.Queries.GetStorageDetailUpdate;

public class GetStorageDetailUpdateQuery : IRequest<StorageDto>
{
    public required int StorageId { get; set; }
}

public class GetStorageDetailUpdateQueryHandler : IRequestHandler<GetStorageDetailUpdateQuery, StorageDto>
{
    private readonly IApplicationDbContext _context;

    public GetStorageDetailUpdateQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StorageDto> Handle(GetStorageDetailUpdateQuery request, CancellationToken cancellationToken)
    {
        var rs = new StorageDto();
        var storage = await _context.Storages
            .Include(s => s.Areas)
            .FirstOrDefaultAsync(s => s.Id == request.StorageId, cancellationToken);

        if (storage != null)
        {
            rs = new StorageDto
            {
                Id = storage.Id,
                Name = storage.Name,
                Location = storage.Location,
                Status = storage.Status.ToString(),
                Areas = storage.Areas?.Select(a => new AreaDto
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToList()
            };
        }

        return rs;
    }
}
