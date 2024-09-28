using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Interfaces;
using warehouse_BE.Application.Customer.Queries.GetListCustomer;

namespace warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerList
{
    public class GetCompanyOwnerListQuery : IRequest<CompanyOwnerListVM>
    {
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
                var customer = await _identityService.GetUsersByRoleAsync("Admin", companyIdResult.CompanyId);
                if (customer.Count > 0)
                {
                    rs.companyOwners = customer;
                }
            }
            return rs;
        }
    }
}
