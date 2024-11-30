﻿using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Linq;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.Users.Queries.GetUserList;

public class GetUserListQuery : IRequest<UserListVm>
{
    public Page? Page { get; set; }
    public string? SearchText { get; set; }
}
public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, UserListVm>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _currentUser;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetUserListQueryHandler(IIdentityService identityService, IUser currentUser, IHttpContextAccessor httpContextAccessor)
    {
        this._identityService = identityService;
        this._currentUser = currentUser;
        this._httpContextAccessor = httpContextAccessor;
    }
    public async Task<UserListVm> Handle(GetUserListQuery request, CancellationToken cancellationToken)
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
                if (!string.IsNullOrEmpty(request.SearchText))
                {
                    data = data.Where(user =>
                        user.Email != null &&
                        user.Email.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }
                rs.Users = data.Skip((request.Page?.PageNumber - 1 ?? 0) * (request.Page?.Size ?? 1))
               .Take(request.Page?.Size ?? 10).ToList();

                if (_httpContextAccessor.HttpContext != null &&
                  _httpContextAccessor.HttpContext.Request != null)
                {
                    var requestContext = _httpContextAccessor.HttpContext.Request;
                    var scheme = requestContext.Scheme;
                    var host = requestContext.Host.Value;
                    rs.Users.ForEach(user =>
                    {
                        user.AvatarImage = $"{scheme}://{host}/Resources/{user.AvatarImage}";
                    });
                }
                rs.Page = new Page
                {
                    Size = request.Page?.Size ?? 0,
                    PageNumber = request.Page?.PageNumber ?? 1,
                    TotalElements = data.Count,
                };
            }
        }
        return rs;
    }
}