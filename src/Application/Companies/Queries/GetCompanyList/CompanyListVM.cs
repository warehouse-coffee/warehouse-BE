using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace warehouse_BE.Application.Companies.Queries.GetCompanyList;

public class CompanyListVM
{
    public List<CompanyDto> CompanyList { get; set; } = new();
}
