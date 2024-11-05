using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Application.Common.Models;
using warehouse_BE.Domain.Common;

namespace warehouse_BE.Application.Employee.Queries.GetListEmployee;

public class EmployeeListVM
{
    public EmployeeListVM()
    {
        Employees = new List<EmployeeDto> { new EmployeeDto() };
    }
   public List<EmployeeDto>? Employees { get; set; }
    public Page? Page { get; set; }
}
