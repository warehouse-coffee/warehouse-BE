using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Common.Models;

namespace warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerList
{
    public class GetCompanyOwnerListQuery : IRequest<CompanyOwnerListVM>
    {
        public Page? Page { get; set; }
    }
    public class GetCompanyOwnerListQueryHandler : IRequestHandler<GetCompanyOwnerListQuery, CompanyOwnerListVM>
    {
        private readonly IIdentityService _identityService;
        private readonly IUser _currentUser;

        public GetCompanyOwnerListQueryHandler(IIdentityService identityService, IUser currentUser)
        {
            _identityService = identityService;
            _currentUser = currentUser;
        }
        public async Task<CompanyOwnerListVM> Handle(GetCompanyOwnerListQuery request, CancellationToken cancellationToken)
        {
            var rs = new CompanyOwnerListVM();
            if (string.IsNullOrEmpty(_currentUser?.Id))
            {
                return rs;
            }
            var companyIdResult = await _identityService.GetCompanyId(_currentUser.Id);
            if (companyIdResult.CompanyId != null)
            {
                var admins = await _identityService.GetUsersByRoleAsync("Admin", companyIdResult.CompanyId);
                if (admins.Count > 0)
                {
                    rs.companyOwners = admins.Skip((request.Page?.PageNumber - 1 ?? 0) * (request.Page?.Size ?? 1))
                      .Take(request.Page?.Size ?? 10).ToList();
                            rs.Page = new Page
                            {
                                Size = request.Page?.Size ?? 0,
                                PageNumber = request.Page?.PageNumber ?? 1,
                                TotalElements = admins.Count,
                            };
                }
            }
            return rs;
        }
    }
}
