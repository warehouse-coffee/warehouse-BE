using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Domain.Common;

namespace warehouse_BE.Application.CompanyOwner.Queries.GetCompanyOwnerList
{
    public class CompanyOwnerListVM
    {
        public CompanyOwnerListVM() {
            companyOwners = new List<UserDto> { new UserDto() };
        }

        public List<UserDto> companyOwners { get; set; }
    }
}
