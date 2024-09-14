using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Storages.Queries.GetStorageList;

public class GetStorageListQuery : IRequest<StorageListVM> { }

public class GetStorageListQueryHandler : IRequestHandler<GetStorageListQuery, StorageListVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetStorageListQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<StorageListVM> Handle(GetStorageListQuery request, CancellationToken cancellationToken)
    {
        var storages = await _context.Storage
            .Include(s => s.Areas) 
            .ToListAsync(cancellationToken);

        var storageDtos = _mapper.Map<List<StorageDto>>(storages); // Map List<Storage> to List<StorageDto>

        return new StorageListVM
        {
            Storages = storageDtos
        };
    }
}