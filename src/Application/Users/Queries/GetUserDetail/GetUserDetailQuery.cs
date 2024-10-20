using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;

namespace warehouse_BE.Application.Users.Queries.GetUserDetail;

public class GetUserDetailQuery : IRequest<UserVM>
{
    public required string userId;
}
public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, UserVM>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetUserDetailQueryHandler(IApplicationDbContext context, IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        this._context = context;
        this._identityService = identityService;
        this._httpContextAccessor = httpContextAccessor;
    }
    public async Task<UserVM> Handle(GetUserDetailQuery request,CancellationToken cancellationToken)
    {
        var rs = new UserVM();
        var data = await _identityService.GetUserById(request.userId);
        if (data != null)
        {
            rs.Id = data.Id;
            rs.Name = data.UserName;
            rs.Email = data.Email;
            rs.Phone = data.PhoneNumber;
            rs.CompanyId = data.CompanyId;

            var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.CompanyId == rs.CompanyId, cancellationToken);

            if (company != null)
            {
                rs.CompanyName = company.CompanyName;
                rs.CompanyPhone = company.PhoneContact;
                rs.CompanyEmail = company.EmailContact;
                rs.CompanyAddress = company.Address;
            }
            if (_httpContextAccessor.HttpContext != null &&
                _httpContextAccessor.HttpContext.Request != null)
            {
                var requestContext = _httpContextAccessor.HttpContext.Request;
                var scheme = requestContext.Scheme;
                var host = requestContext.Host.Value;
                rs.AvatarImage = $"{scheme}://{host}/Resources/{data.AvatarImage}";
            }

        }
        return rs;
    }
}
