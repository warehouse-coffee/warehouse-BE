using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Users.Queries.GetUserList;

public class GetUserListQuery : IRequest<UserListVm>
{
}
public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, UserListVm>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;
    public GetUserListQueryHandler(IIdentityService identityService, IUser currentUser)
    {
        this._identityService = identityService;
        this._currentUser = currentUser;
    }
    public async Task<UserListVm> Handle(GetUserListQuery reques, CancellationToken cancellationToken)
    {
        var rs = new UserListVm();
        if (string.IsNullOrEmpty(_currentUser?.Id))
        {
            return rs;
        }
        var userrole = await _identityService.IsInRoleAsync(_currentUser.Id, "Super-Admin");
        if (userrole)
        {
            var data = await _identityService.GetUserList();
            if (data != null)
            {
                rs.Users = data;
            }
        }
        return rs;
    }
}