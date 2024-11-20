using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Storages.Queries.GetStorageOfUser;

namespace warehouse_BE.Application.Storages.Queries.GetListISorageInfoOfUser;

public class GetListISorageInfoOfUserQuery : IRequest<ListISorageInfoOfUserVM>
{
}
public class GetListISorageInfoOfUserQueryHandler : IRequestHandler<GetListISorageInfoOfUserQuery, ListISorageInfoOfUserVM>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _currentUser;
    private readonly ILoggerService _logger;
    public GetListISorageInfoOfUserQueryHandler(IIdentityService identityService, IApplicationDbContext context, IMapper mapper, IUser currentUser, ILoggerService logger)
    {
        this._identityService = identityService;
        this._context = context;
        this._mapper = mapper;
        this._currentUser = currentUser;
        this._logger = logger;
    }
    public async Task<ListISorageInfoOfUserVM> Handle(GetListISorageInfoOfUserQuery request, CancellationToken cancellationToken)
    {
        var rs = new ListISorageInfoOfUserVM();
        try
        {
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                return rs;
            }
            var storages = await _identityService.GetUserStoragesAsync(_currentUser.Id);

            var storageDtos = _mapper.Map<List<StorageName>>(storages);

            var pagedStorages = storageDtos
                .ToList();

            rs.Storages = pagedStorages;
        } catch (Exception ex)
        {
            _logger.LogError("Error at get list Storage Info of User", ex);
        }
      

        return rs;
    }
}
