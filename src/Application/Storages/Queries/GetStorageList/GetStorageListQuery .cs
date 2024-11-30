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
    private readonly IUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly ILoggerService _loggerService;
    public GetStorageListQueryHandler(IApplicationDbContext context, IMapper mapper,IUser currentUser, IIdentityService identityService, ILoggerService loggerService)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _identityService = identityService;
        _loggerService = loggerService;
    }

    public async Task<StorageListVM> Handle(GetStorageListQuery request, CancellationToken cancellationToken)
    {
        var rs = new StorageListVM();
        try
        {
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                return rs;
            }
            var storages = await _identityService.GetUserStoragesAsync(_currentUser.Id);
            var storagesId = storages.Select(x => x.Id).ToList();
            var storageDtos = _mapper.Map<List<StorageDto>>(_context.Storages.Include(o => o.Areas).Where(o => !o.IsDeleted && storagesId.Contains(o.Id)));
            if(storageDtos.Any())
            {
                rs.Storages = storageDtos;
            }
        }
        catch(Exception ex) 
        {
            _loggerService.LogError("Error at GetStorageListQuery", ex);
        }
        return rs;
    }
}