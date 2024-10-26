using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;
using warehouse_BE.Application.Response;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Storages.Queries.GetStoragebyCompanyId;

public class GetStorageOfUserQuery : IRequest<UserStorageList>
{
    public Page? Page { get; set; }
}
public class GetListStoragebyCompanyIdQueryHandler : IRequestHandler<GetStorageOfUserQuery, UserStorageList>
{
    private readonly IIdentityService _identityService;     
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUser _currentUser;
    private readonly ILoggerService _logger;
    public GetListStoragebyCompanyIdQueryHandler(IIdentityService identityService, IApplicationDbContext context, IMapper mapper, IUser currentUser, ILoggerService logger)
    {
        this._identityService = identityService;
        this._context = context;
        this._mapper = mapper;
        this._currentUser = currentUser;
        this._logger = logger;
    }
    public async Task<UserStorageList> Handle(GetStorageOfUserQuery request, CancellationToken cancellationToken)
    {
        var rs = new UserStorageList();
        if (string.IsNullOrEmpty(_currentUser?.Id))
        {
            return rs;
        }
        var storages = await _identityService.GetUserStoragesAsync(_currentUser.Id);

        var storageDtos = _mapper.Map<List<StorageDto>>(storages);

        var pagedStorages = storageDtos
            .Skip((request.Page?.PageNumber - 1 ?? 0) * (request.Page?.Size ?? 1))
            .Take(request.Page?.Size ?? 10)
            .ToList();

        rs.Storages = pagedStorages;

        rs.Page = new Page
        {
            Size = request.Page?.Size ?? 0,
            PageNumber = request.Page?.PageNumber ?? 1,
            TotalElements = storageDtos.Count 
        };

        return rs;
    }

}
