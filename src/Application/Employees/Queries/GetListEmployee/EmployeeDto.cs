using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_BE.Domain.Entities;

namespace warehouse_BE.Application.Employee.Queries.GetListEmployee;

public class EmployeeDto
{
    public string? Id { get; set; }
    public string? CompanyId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool isActived { get; set; }
    public string? AvatarImage { get; set; }
}
